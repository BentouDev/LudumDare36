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

    struct WorldInfo
    {
        public int RoomsLeft;
        public bool HasBoss;
    }

    public Transform WorldParent;

    public GameObject FirstRoom;
    public List<RoomInfo> RoomPrefabs;

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

        result.RoomCount = (int) (result.WorldHeight * result.WorldWidth * fullfill);
        
        result.RoomMap = new WorldData.RoomCell[result.WorldWidth][];
        for (int i = 0; i < result.WorldWidth; i++)
        {
            result.RoomMap[i] = new WorldData.RoomCell[result.WorldHeight];
        }

        var xPos = Random.Range(0, result.WorldWidth - 1);
        var yPos = Random.Range(0, result.WorldHeight - 1);

        Info.RoomsLeft = result.RoomCount;

        result.FirstRoom = BuildRoomsRecurrent(ref result, new Room.MapPos(xPos, yPos));
        
        return result;
    }

    private Room BuildRoomsRecurrent(ref WorldData world, Room.MapPos pos)
    {
        RoomInfo roomInfo = Info.RoomsLeft != world.RoomCount ? RandomizePrefab(ref world) 
            : new RoomInfo() {Prefab = FirstRoom, Type = Room.RoomType.Normal};
        
        var go = Instantiate(roomInfo.Prefab, WorldParent) as GameObject;
        var room = go.GetComponent<Room>();
            room.MapPosition = pos;
            room.gameObject.SetActive(false);

        world.RoomMap[pos.x][pos.y] = new WorldData.RoomCell(roomInfo.Type, room);
        world.AllRooms.Add(room);

        if (Info.RoomsLeft > 0 && !Info.HasBoss)
        {
            int possibleCount;
            var potentialDoors = CheckFreeConnections(ref world, out possibleCount, room.MapPosition);
            int roomCount = Random.Range(1, Mathf.Min(possibleCount, Info.RoomsLeft));
            
            IList<int> directionList = new List<int>() {0,1,2,3};

            while (roomCount > 0 && potentialDoors != Room.DoorDirection.None)
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
                            BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(-1, 0));
                        }
                        break;
                    case 1:
                        if ((potentialDoors & Room.DoorDirection.Right) != 0)
                        {
                            potentialDoors &= ~Room.DoorDirection.Right;
                            roomCount--;
                            Info.RoomsLeft--;
                            directionList.RemoveAt(index);
                            BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(1, 0));
                        }
                        break;
                    case 2:
                        if ((potentialDoors & Room.DoorDirection.Down) != 0)
                        {
                            potentialDoors &= ~Room.DoorDirection.Down;
                            roomCount--;
                            Info.RoomsLeft--;
                            directionList.RemoveAt(index);
                            BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, -1));
                        }
                        break;
                    case 3:
                        if ((potentialDoors & Room.DoorDirection.Up) != 0)
                        {
                            potentialDoors &= ~Room.DoorDirection.Up;
                            roomCount--;
                            Info.RoomsLeft--;
                            directionList.RemoveAt(index);
                            BuildRoomsRecurrent(ref world, room.MapPosition + new Room.MapPos(0, 1));
                        }
                        break;
                }
            }
        }

        return room;
    }
    
    private Room.DoorDirection CheckFreeConnections(ref WorldData world, out int count, Room.MapPos pos)
    {
        Room.DoorDirection result = Room.DoorDirection.None;
        count = 0;

        if (pos.x - 1 >= 0 && world.RoomMap[pos.x - 1][pos.y].Type == Room.RoomType.Empty)
        {
            result |= Room.DoorDirection.Left;
            count++;
        }
        if (pos.x + 1 < world.WorldWidth && world.RoomMap[pos.x + 1][pos.y].Type == Room.RoomType.Empty)
        {
            result |= Room.DoorDirection.Right;
            count++;
        }
        if (pos.y - 1 >= 0 && world.RoomMap[pos.x][pos.y - 1].Type == Room.RoomType.Empty)
        {
            result |= Room.DoorDirection.Down;
            count++;
        }
        if (pos.y + 1 < world.WorldHeight && world.RoomMap[pos.x][pos.y + 1].Type == Room.RoomType.Empty)
        {
            result |= Room.DoorDirection.Up;
            count++;
        }

        return result;
    }

    private RoomInfo RandomizePrefab(ref WorldData world)
    {
        RoomInfo result;

        if (!Info.HasBoss && Info.RoomsLeft < 0.5f * world.RoomCount)
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
