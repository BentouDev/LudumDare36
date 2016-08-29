using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct RoomInfo
    {
        [SerializeField]
        public Room.RoomType Type;

        [SerializeField]
        public GameObject Prefab;
    }

    class KeyInfo
    {
        public Door.Key Key;
        public bool HasLock;
        public bool HasPickup;
    }

    struct WorldInfo
    {
        public int RoomsLeft;
        public Stack<KeyInfo> Keys;
        public int KeysCount;
        public bool HasBoss;
    }

    public bool SetSeed;
    public Random.State Seed;

    public Transform WorldParent;

    public GameObject FirstRoom;
    public List<RoomInfo> RoomPrefabs;

    public List<Door.Key> Keys;

    public int MinWidth = 3;
    public int MaxWidth = 10;

    public int MinHeight = 3;
    public int MaxHeight = 10;

    public int CorridorLength = 4;

    [Range(0.125f, 0.75f)]
    public float MinMapFullfill = 0.3f;

    [Range(0.125f, 0.75f)]
    public float MaxMapFullfill = 0.7f;

    public int MinCount { get { return MinWidth * MinHeight; } }

    public int MaxCount { get { return MaxWidth * MaxHeight; } }

    private WorldInfo Info;

    public WorldData GenerateWorld()
    {
        if (SetSeed)
            Random.state = Seed;

        Debug.Log("SEED : " + Random.state);

        WorldData result = new WorldData();
        Info = new WorldInfo();

        result.WorldHeight = Mathf.CeilToInt(Random.Range(MinHeight, MaxHeight));
        result.WorldWidth = Mathf.CeilToInt(Random.Range(MinWidth, MaxWidth));

        var fullfill = Random.Range(MinMapFullfill, MaxMapFullfill);

        result.RoomCount = Mathf.CeilToInt((result.WorldHeight * result.WorldWidth) * fullfill);
        
        result.RoomMap = new WorldData.RoomCell[result.WorldWidth][];
        for (int i = 0; i < result.WorldWidth; i++)
        {
            result.RoomMap[i] = new WorldData.RoomCell[result.WorldHeight];
        }

        var xPos = Random.Range(0, result.WorldWidth - 1);
        var yPos = Random.Range(0, result.WorldHeight - 1);

        Info.RoomsLeft = result.RoomCount;
        Info.KeysCount = Mathf.Clamp(Mathf.CeilToInt((float)result.RoomCount / (float)Keys.Count), 0, Keys.Count);
        Info.Keys = new Stack<KeyInfo>();

        result.FirstRoom = BuildRoomsRecurrent(ref result, new Room.MapPos(xPos, yPos), CorridorLength).Reference;

        BuildLocks(result);

        Debug.Log("Rooms : " + result.RoomCount + " vs " + result.RoomMap.SelectMany(c => c).Count(c => c.Type != Room.RoomType.Empty));
        Debug.Log("Keys : " + Info.KeysCount + " vs " + result.RoomMap.SelectMany(c => c).Count(c => c.Reference != null && c.Reference.KeyPickup != null));
        Debug.Log("Rooms Left : " + Info.RoomsLeft);
        var bossPos = result.RoomMap.SelectMany(c => c).First(c => c.Type == Room.RoomType.Boss).Reference.MapPosition;
        Debug.Log("Boss : " + Info.HasBoss + " at " + bossPos.x + " :: " + bossPos.y);
        
        return result;
    }

    struct FindResult
    {
        public Room Room;
        public int Distance;
    }

    private FindResult FindFurthest(WorldData.RoomCell cell, ref List<Room> processed, int distance = 0)
    {
        var thisDistance = distance + 1;
        var result = new FindResult() { Room = cell.Reference, Distance = thisDistance };
        
        processed.Add(cell.Reference);

        foreach (var connection in cell.Reference.Connections)
        {
            if(!processed.Contains(connection.Value.Reference) && connection.Value.Reference.KeyLock == null)
            {
                var next = FindFurthest(connection.Value, ref processed, thisDistance);
                if (next.Distance > result.Distance)
                {
                    result = next;
                }
            }
        }

        return result;
    }

    private void BuildLocks(WorldData data)
    {
        var firstCell = data.RoomMap[data.FirstRoom.MapPosition.x][data.FirstRoom.MapPosition.y];

        List<Room> processed = new List<Room>();
        List<WorldData.RoomCell> rooms = new List<WorldData.RoomCell>();

        ProcessLocks(data.FirstRoom, ref processed, ref rooms);
        rooms.Add(firstCell);

        processed.Clear();

        for (int i = 0; i < Info.KeysCount; i++)
        {
            var key = Keys[i];
            Info.Keys.Push(new KeyInfo() { Key = key });
        }

        var bossCell = rooms[0];
        var furthest = FindFurthest(bossCell, ref processed, 0);        
        var bossKey = new Door.Key() {color = Color.red};

        bossCell.Reference.KeyLock = bossKey;
        furthest.Room.KeyPickup = bossKey;

        List<Room> locked = new List<Room>();
        while (Info.Keys.Any())
        {
            processed.Clear();
            processed.AddRange(locked);

            var nextKey = Info.Keys.Pop();
            var next = FindFurthest(bossCell, ref processed);

            if (furthest.Room.KeyLock != null
            || next.Room.KeyPickup != null)
                continue;

            furthest.Room.KeyLock = nextKey.Key;
            next.Room.KeyPickup = nextKey.Key;

            locked.Add(furthest.Room);

            furthest = next;
        }
    }
    
    private bool ProcessLocks(Room cell, ref List<Room> processed, ref List<WorldData.RoomCell> list)
    {
        bool result = false;

        processed.Add(cell);

        foreach (var connection in cell.Connections)
        {
            if (processed.Contains(connection.Value.Reference))
                continue;

            if (connection.Value.Type == Room.RoomType.Boss)
            {
                list.Add(connection.Value);
                result = true;

                break;
            }
            else
            {
                result = ProcessLocks(connection.Value.Reference, ref processed, ref list);
                if (result)
                {
                    list.Add(connection.Value);
                }
            }
        }

        return result;
    }

    private void ProcessRooms(ref WorldData world, WorldData.RoomCell cell, int length)
    {
        var room = cell.Reference;
        room.IsMainRoom = !Info.HasBoss;

        length = room.IsMainRoom ? CorridorLength : length - 1;

        int possibleCount;
        var potentialDoors = CheckFreeConnections(ref world, out possibleCount, cell);
        int roomCount = Mathf.Min(possibleCount, Info.RoomsLeft);
        
        IList<int> directionList = new List<int>() { 0, 1, 2, 3 };

        while (roomCount > 0 && Info.RoomsLeft > 0 && potentialDoors != Room.DoorDirection.None && length > 0)
        {
            int index = Random.Range(0, directionList.Count);
            int dir = directionList[index];

            switch (dir)
            {
            case 0:
                if ((potentialDoors & Room.DoorDirection.Left) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Left;
                        
                    roomCount--;
                    Info.RoomsLeft--;
                    directionList.RemoveAt(index);
                        
                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(-1, 0), length);
                    //newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Left], cell);
                    room.SetConnection(Room.DoorDirection.Left, newRoom, cell);
                }
                break;
            case 1:
                if ((potentialDoors & Room.DoorDirection.Right) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Right;
                        
                    roomCount--;
                    Info.RoomsLeft--;
                    directionList.RemoveAt(index);

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(1, 0), length);
                    //newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Right], cell);
                    room.SetConnection(Room.DoorDirection.Right, newRoom, cell);
                }
                break;
            case 2:
                if ((potentialDoors & Room.DoorDirection.Down) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Down;
                        
                    roomCount--;
                    Info.RoomsLeft--;
                    directionList.RemoveAt(index);

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, -1), length);
                    //newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Down], cell);
                    room.SetConnection(Room.DoorDirection.Down, newRoom, cell);
                }
                break;
            case 3:
                if ((potentialDoors & Room.DoorDirection.Up) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Up;
                        
                    roomCount--;
                    Info.RoomsLeft--;
                    directionList.RemoveAt(index);

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, 1), length);
                    //newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Up], cell);
                    room.SetConnection(Room.DoorDirection.Up, newRoom, cell);
                }
                break;
            }
        }
    }

    private WorldData.RoomCell BuildRoomsRecurrent(ref WorldData world, Room.MapPos pos, int length)
    {
        RoomInfo roomInfo = Info.RoomsLeft != world.RoomCount ? RandomizePrefab(ref world) 
            : new RoomInfo() {Prefab = FirstRoom, Type = Room.RoomType.Normal};
        
        var go = Instantiate(roomInfo.Prefab, WorldParent) as GameObject;
        var room = go.GetComponent<Room>();
            room.MapPosition = pos;
            room.gameObject.SetActive(false);

        var cell = new WorldData.RoomCell(roomInfo.Type, room);

        /*if (roomInfo.Type == Room.RoomType.Boss)
        {
            room.KeyLock = new Door.Key() {color = Color.red};
            Info.Keys.Push(new KeyInfo() { Key = room.KeyLock, HasLock = true});

            for (int i = 0; i < Info.KeysCount; i++)
            {
                var key = Keys[i];

                Info.Keys.Push(new KeyInfo() { Key = key });
            }
        }*/

        world.RoomMap[pos.x][pos.y] = cell;
        world.AllRooms.Add(room);
        
        if (Info.RoomsLeft > 0 && roomInfo.Type != Room.RoomType.Boss)
        {
            ProcessRooms(ref world, cell, length);
        }

        /*if (Info.HasBoss
        && roomInfo.Type != Room.RoomType.Boss
        && room != world.FirstRoom
        && Info.Keys.Any())
        {
            var keyInfo = Info.Keys.Peek();
            if (!keyInfo.HasPickup)
            {
                keyInfo.HasPickup = true;
                room.KeyPickup = keyInfo.Key;
            }

            if (!keyInfo.HasLock && !room.IsMainRoom)
            {
                keyInfo.HasLock = true;
                room.KeyLock = keyInfo.Key;
            }

            if (keyInfo.HasLock && keyInfo.HasPickup)
            {
                Info.Keys.Pop();
            }
        }*/

        return cell;
    }
    
    private Room.DoorDirection CheckFreeConnections(ref WorldData world, out int count, WorldData.RoomCell cell)
    {
        Room.DoorDirection result = Room.DoorDirection.None;
        count = 0;

        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            var room = cell.Reference.GetGlobalRoomCell(world, dir);

            if (room.Type == Room.RoomType.Empty
            && !cell.Reference.HasEdge(world, dir))
            {
                result |= dir;
                count++;
            }
        }

        return result;
    }

    private RoomInfo RandomizePrefab(ref WorldData world)
    {
        RoomInfo result;

        bool random = !Info.HasBoss && Info.RoomsLeft < 0.75f * world.RoomCount;
        // bool emergency = !Info.HasBoss && Info.RoomsLeft == 1;

        if (random)
        {
            Info.HasBoss = true;
            result = RoomPrefabs.First(rm => rm.Type == Room.RoomType.Boss);
        }
        else
        {
            var normal = RoomPrefabs.Where(rm => rm.Type == Room.RoomType.Normal);
            var roomInfos = normal as RoomInfo[] ?? normal.ToArray();
            int index = Random.Range(0, roomInfos.Length);

            result = roomInfos[index];
        }

        return result;
    }
}
