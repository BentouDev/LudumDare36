using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    public Transform WorldParent;

    public GameObject FirstRoom;
    public List<RoomInfo> RoomPrefabs;

    public List<Door.Key> Keys;

    public int MinWidth = 3;
    public int MaxWidth = 10;

    public int MinHeight = 3;
    public int MaxHeight = 10;

    [Range(0.125f, 0.75f)]
    public float MinMapFullfill = 0.3f;

    [Range(0.125f, 0.75f)]
    public float MaxMapFullfill = 0.7f;

    public int MinCount { get { return MinWidth * MinHeight; } }

    public int MaxCount { get { return MaxWidth * MaxHeight; } }

    private WorldInfo Info;

    public WorldData GenerateWorld()
    {
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
        Info.KeysCount = Mathf.Clamp(Mathf.FloorToInt((float)result.RoomCount / (float)Keys.Count), 0, Keys.Count);
        Info.Keys = new Stack<KeyInfo>();

        result.FirstRoom = BuildRoomsRecurrent(ref result, new Room.MapPos(xPos, yPos)).Reference;

        Debug.Log("Rooms : " + result.RoomCount + " vs " + result.RoomMap.SelectMany(c => c).Count(c => c.Type != Room.RoomType.Empty));
        Debug.Log("Rooms Left : " + Info.RoomsLeft);
        Debug.Log("Keys : " + Info.Keys.Count);
        
        return result;
    }

    private void ProcessRooms(ref WorldData world, WorldData.RoomCell cell)
    {
        var room = cell.Reference;
        room.IsMainRoom = !Info.HasBoss;

        int possibleCount;
        var potentialDoors = CheckFreeConnections(ref world, out possibleCount, cell);
        int roomCount = Mathf.Min(possibleCount, Info.RoomsLeft); //  Random.Range(1, 

        IList<int> directionList = new List<int>() { 0, 1, 2, 3 };

        while (roomCount > 0 && Info.RoomsLeft > 0 && potentialDoors != Room.DoorDirection.None)
        {
            int index = Random.Range(0, directionList.Count);
            int dir = directionList[index];

            roomCount--;
            Info.RoomsLeft--;
            directionList.RemoveAt(index);

            switch (dir)
            {
            case 0:
                if ((potentialDoors & Room.DoorDirection.Left) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Left;

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(-1, 0));
                    newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Left], cell);
                    room.SetConnection(Room.DoorDirection.Left, newRoom);
                }
                break;
            case 1:
                if ((potentialDoors & Room.DoorDirection.Right) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Right;

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(1, 0));
                    newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Right], cell);
                    room.SetConnection(Room.DoorDirection.Right, newRoom);
                }
                break;
            case 2:
                if ((potentialDoors & Room.DoorDirection.Down) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Down;

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, -1));
                    newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Down], cell);
                    room.SetConnection(Room.DoorDirection.Down, newRoom);
                }
                break;
            case 3:
                if ((potentialDoors & Room.DoorDirection.Up) != 0)
                {
                    potentialDoors &= ~Room.DoorDirection.Up;

                    var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, 1));
                    newRoom.Reference.SetConnection(Room.InverseDirection[Room.DoorDirection.Up], cell);
                    room.SetConnection(Room.DoorDirection.Up, newRoom);
                }
                break;
            }
        }
    }

    private WorldData.RoomCell BuildRoomsRecurrent(ref WorldData world, Room.MapPos pos)
    {
        RoomInfo roomInfo = Info.RoomsLeft != world.RoomCount ? RandomizePrefab(ref world) 
            : new RoomInfo() {Prefab = FirstRoom, Type = Room.RoomType.Normal};
        
        var go = Instantiate(roomInfo.Prefab, WorldParent) as GameObject;
        var room = go.GetComponent<Room>();
            room.MapPosition = pos;
            room.gameObject.SetActive(false);

        var cell = new WorldData.RoomCell(roomInfo.Type, room);

        if (roomInfo.Type == Room.RoomType.Boss)
        {
            room.KeyLock = new Door.Key() {color = Color.red};
            Info.Keys.Push(new KeyInfo() { Key = room.KeyLock, HasLock = true});

            for (int i = 0; i < Info.KeysCount; i++)
            {
                var key = Keys[i];

                Info.Keys.Push(new KeyInfo() { Key = key });
            }
        }

        world.RoomMap[pos.x][pos.y] = cell;
        world.AllRooms.Add(room);

        if (Info.RoomsLeft == 0 && !Info.HasBoss)
        {
            Debug.LogError("Run out of rooms before boos was spawned!");
            // Info.RoomsLeft++;
        }

        if (Info.RoomsLeft > 0 && roomInfo.Type != Room.RoomType.Boss)
        {
            ProcessRooms(ref world, cell);
        }

        if (Info.HasBoss
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
        }

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
