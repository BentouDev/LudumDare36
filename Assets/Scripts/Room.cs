using UnityEngine;
using System.Collections.Generic;

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

    public MapPos MapPosition;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, RoomSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green + Color.gray;
        Gizmos.DrawWireCube(transform.position, RoomSize);
    }
    
    public bool HasRoom(WorldData data, Room.DoorDirection direction)
    {
        WorldData.RoomCell cell = GetRoomCell(data, direction);
        return cell.Type != RoomType.Empty;
    }

    public WorldData.RoomCell GetRoomCell(WorldData data, Room.DoorDirection direction)
    {
        var offset = RoomOffset[direction];
        var pos = MapPosition + offset;
        
        if(pos.x < 0 || pos.x >= data.WorldWidth || pos.y < 0 || pos.y >= data.WorldHeight)
            return new WorldData.RoomCell();

        WorldData.RoomCell result = data.RoomMap[pos.x][pos.y];
        
        return result;
    }
}
