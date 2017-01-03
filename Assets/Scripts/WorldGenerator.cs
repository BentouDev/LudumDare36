using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WorldGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Debug/Save Last Random Seed")]
    public static void SaveSeed()
    {
        var json = JsonUtility.ToJson(LastSeed);
        PlayerPrefs.SetString("LastRandom", json);
    }
#endif

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

    [Header("Debug")]
    public bool SetSavedSeed;

#if UNITY_EDITOR
    [SerializeField]
    private static Random.State LastSeed;
#endif

    [Header("Rooms")]
    public Transform WorldParent;

    [Space]
    public GameObject FirstRoom;
    public List<RoomInfo> RoomPrefabs;

    [Header("Keys")]
    public List<Door.Key> Keys;

    [Header("World Size")]
    public int CorridorLength = 4;
    public bool allowLoops;

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
#if UNITY_EDITOR
        if (SetSavedSeed)
        {
            var json = PlayerPrefs.GetString("LastRandom");
            if (!string.IsNullOrEmpty(json))
            {
                LastSeed = JsonUtility.FromJson<Random.State>(json);
                Random.state = LastSeed;
            }
        }
        else
        {
            LastSeed = Random.state;
        }
#endif

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

            var next = FindFurthest(bossCell, ref processed);

            if (furthest.Room != next.Room
            && furthest.Room.KeyLock == null
            && next.Room.KeyPickup == null 
            && furthest.Room != data.FirstRoom
            && next.Room != data.FirstRoom
            && !furthest.Room.IsMainRoom
            && !next.Room.IsMainRoom)
            {
                var nextKey = Info.Keys.Pop();
                    furthest.Room.KeyLock = nextKey.Key;
                    next.Room.KeyPickup = nextKey.Key;
            }

            if(!locked.Contains(furthest.Room))
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

    private void BuildChildRooms(ref WorldData world, WorldData.RoomCell cell, int length)
    {
        var room = cell.Reference;
        room.IsMainRoom = !Info.HasBoss;

        length = room.IsMainRoom ? CorridorLength : length - 1;

        int possibleCount;
        List<Room.DoorDirection> directions = CheckFreeConnections(ref world, out possibleCount, cell);
        int roomCount = Mathf.Min(possibleCount, Info.RoomsLeft);

        while (roomCount > 0 && Info.RoomsLeft > 0 && directions.Any() && length > 0)
        {
            int index = Random.Range(0, directions.Count);
            Room.DoorDirection dir = directions[index];
            
            roomCount--;
            Info.RoomsLeft--;

            directions.RemoveAt(index);

            var newRoom = BuildRoomsRecurrent(ref world, room.MapPosition + Room.RoomOffset[dir], length);
            room.SetConnection(dir, newRoom, cell);

            if (!allowLoops)
            {
                directions = CheckFreeConnections(ref world, out possibleCount, cell);
                roomCount = Mathf.Min(possibleCount, Info.RoomsLeft);
            }
        }
    }

    private WorldData.RoomCell BuildRoomsRecurrent(ref WorldData world, Room.MapPos pos, int length)
    {
        var cell = world.RoomMap[pos.x][pos.y];
        if (cell.IsEmpty())
        {
            RoomInfo roomInfo = Info.RoomsLeft != world.RoomCount ? RandomizePrefab(ref world)
                : new RoomInfo() { Prefab = FirstRoom, Type = Room.RoomType.Normal };

            var go = Instantiate(roomInfo.Prefab, WorldParent) as GameObject;
            var room = go.GetComponent<Room>();
                room.MapPosition = pos;
                room.gameObject.SetActive(false);

            cell = new WorldData.RoomCell(roomInfo.Type, room);

            world.RoomMap[pos.x][pos.y] = cell;
            world.AllRooms.Add(room);
        }

        if (Info.RoomsLeft > 0 && cell.Type != Room.RoomType.Boss)
        {
            BuildChildRooms(ref world, cell, length);
        }

        return cell;
    }
    
    private List<Room.DoorDirection> CheckFreeConnections(ref WorldData world, out int count, WorldData.RoomCell cell)
    {
        List<Room.DoorDirection> result = new List<Room.DoorDirection>();
        count = 0;

        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            var room = cell.Reference.GetGlobalRoomCell(world, dir);

            if (room.Type == Room.RoomType.Empty
            && !cell.Reference.HasEdge(world, dir))
            {
                result.Add(dir);
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
