using UnityEngine;
using System.Collections.Generic;

public class WorldData
{
    public WorldData()
    {
        AllRooms = new List<Room>();
    }

    public Room FirstRoom;

    public Room LastRoom;

    public List<Room> AllRooms;

    public int RoomCount;

    public RoomCell[][] RoomMap;

    public int WorldWidth;

    public int WorldHeight;

    [System.Serializable]
    public struct RoomCell
    {
        [SerializeField]
        public Room.RoomType Type;

        [SerializeField]
        public Room Reference;
        
        public RoomCell(Room.RoomType type, Room room)
        {
            Type = type;
            Reference = room;
        }
    }
}
