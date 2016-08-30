using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public enum RoomType
    {
        Empty = 0,
        Normal = 1,
        Boss = 2
    }

    [System.Serializable]
    [System.Flags]
    public enum DoorDirection
    {
        None = 0,
        Left = 1,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3
    }

    public static readonly Dictionary<Room.DoorDirection, Room.DoorDirection> InverseDirection =
    new Dictionary<Room.DoorDirection, Room.DoorDirection>() {
        {Room.DoorDirection.Left, Room.DoorDirection.Right},
        {Room.DoorDirection.Right, Room.DoorDirection.Left},
        {Room.DoorDirection.Up, Room.DoorDirection.Down},
        {Room.DoorDirection.Down, Room.DoorDirection.Up},
    };

    public static readonly Dictionary<DoorDirection, MapPos> RoomOffset =
    new Dictionary<DoorDirection, MapPos>()
    {
        {Room.DoorDirection.Left, new MapPos(-1, 0)},
        {Room.DoorDirection.Right, new MapPos(1, 0)},
        {Room.DoorDirection.Up, new MapPos(0, 1)},
        {Room.DoorDirection.Down, new MapPos(0, -1)},
    };

    public static readonly Dictionary<DoorDirection, Vector3> DoorForward =
    new Dictionary<DoorDirection, Vector3>()
    {
            {Room.DoorDirection.Left, Vector3.left},
            {Room.DoorDirection.Right, Vector3.right},
            {Room.DoorDirection.Up, Vector3.forward},
            {Room.DoorDirection.Down, Vector3.back},
    };

    public static readonly DoorDirection[] AllDirs =
    {
        Room.DoorDirection.Left,
        Room.DoorDirection.Right,
        Room.DoorDirection.Up,
        Room.DoorDirection.Down,
    };

    [System.Serializable]
    public struct MapPos
    {
        [SerializeField]
        public int x;

        [SerializeField]
        public int y;

        public MapPos(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }

        public static MapPos operator+(MapPos first, MapPos second)
        {
            return new MapPos(first.x + second.x, first.y + second.y);
        }

        public static MapPos operator -(MapPos first, MapPos second)
        {
            return new MapPos(first.x - second.x, first.y - second.y);
        }
    }

    public Vector3 RoomSize = Vector3.one;

    public Transform LeftDoor;
    public Transform RightDoor;
    public Transform DownDoor;
    public Transform UpDoor;

    private List<Enemy> Enemies;

    public Dictionary<DoorDirection, WorldData.RoomCell> Connections = new Dictionary<DoorDirection, WorldData.RoomCell>();

    public bool IsMainRoom { get; set; }
    public Door.Key KeyLock { get; set; }
    public Door.Key KeyPickup { get; set; }
    public MapPos MapPosition { get; set; }

    public bool IsCleared {  get { return !Enemies.Any() || !Enemies.Any(e => e.IsAlive); } }
    public bool IsDiscovered { get; set; }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, RoomSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green + Color.gray;
        Gizmos.DrawWireCube(transform.position, RoomSize);

        Gizmos.color = Color.red;

        if(LeftDoor)
            Gizmos.DrawSphere(LeftDoor.position, 0.25f);

        if (RightDoor)
            Gizmos.DrawSphere(RightDoor.position, 0.25f);

        if (DownDoor)
            Gizmos.DrawSphere(DownDoor.position, 0.25f);

        if (UpDoor)
            Gizmos.DrawSphere(UpDoor.position, 0.25f);
    }
    
    public bool HasRoom(WorldData data, Room.DoorDirection direction)
    {
        WorldData.RoomCell cell = GetGlobalRoomCell(data, direction);
        return cell.Type != RoomType.Empty;
    }

    public bool HasEdge(WorldData data, DoorDirection direction)
    {
        var offset = RoomOffset[direction];
        var pos = MapPosition + offset;

        if (pos.x < 0 || pos.x >= data.WorldWidth || pos.y < 0 || pos.y >= data.WorldHeight)
            return true;

        return false;
    }

    public void SetConnection(DoorDirection direction, WorldData.RoomCell cell, WorldData.RoomCell thisCell)
    {
        if (cell.Type == RoomType.Empty || cell.Reference == null)
            return;

        Connections[direction] = cell;
        cell.Reference.Connections[InverseDirection[direction]] = thisCell;
    }

    public WorldData.RoomCell GetConnectedRoomCell(DoorDirection direction)
    {
        WorldData.RoomCell result = new WorldData.RoomCell();

        if (Connections.ContainsKey(direction))
            result = Connections[direction];

        return result;
    }

    public WorldData.RoomCell GetGlobalRoomCell(WorldData data, Room.DoorDirection direction)
    {
        var offset = RoomOffset[direction];
        var pos = MapPosition + offset;

        if(HasEdge(data, direction))
            return new WorldData.RoomCell();

        WorldData.RoomCell result = data.RoomMap[pos.x][pos.y];
        
        return result;
    }

    public void ShowAllDoorPlaceholders()
    {
        if(LeftDoor && !LeftDoor.gameObject.activeSelf)
            LeftDoor.gameObject.SetActive(true);

        if (RightDoor && !RightDoor.gameObject.activeSelf)
            RightDoor.gameObject.SetActive(true);

        if (DownDoor && !DownDoor.gameObject.activeSelf)
            DownDoor.gameObject.SetActive(true);

        if (UpDoor && !UpDoor.gameObject.activeSelf)
            UpDoor.gameObject.SetActive(true);
    }

    public Vector3 GetDoorPosition(DoorDirection dir)
    {
        Vector3 result = Vector3.zero;
        
        switch (dir)
        {
            case DoorDirection.Left:
                if (LeftDoor)
                    result = LeftDoor.transform.position;
                break;
            case DoorDirection.Right:
                if (RightDoor)
                    result = RightDoor.transform.position;
                break;
            case DoorDirection.Down:
                if (DownDoor)
                    result = DownDoor.transform.position;
                break;
            case DoorDirection.Up:
                if (UpDoor)
                    result = UpDoor.transform.position;
                break;
        }

        return result;
    }

    public void HideDoorPlaceholder(DoorDirection dir)
    {
        switch (dir)
        {
        case DoorDirection.Left:
            if(LeftDoor)
                LeftDoor.gameObject.SetActive(false);
            break;
        case DoorDirection.Right:
            if(RightDoor)
                RightDoor.gameObject.SetActive(false);
            break;
        case DoorDirection.Down:
            if(DownDoor)
                DownDoor.gameObject.SetActive(false);
            break;
        case DoorDirection.Up:
            if(UpDoor)
                UpDoor.gameObject.SetActive(false);
            break;
        }
    }

    public void AwakeEnemies(Pawn pawn)
    {
        Enemies = GetComponentsInChildren<Enemy>().ToList();
        foreach (var enemy in Enemies)
        {
            enemy.OnStart(pawn);
        }
    }
}
