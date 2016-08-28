using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CellIcon : MonoBehaviour
{
    public Image Content;
    public Image Left;
    public Image Right;
    public Image Down;
    public Image Up;

    public WorldData.RoomCell Cell;

    public Image GetImage(Room.DoorDirection dir)
    {
        switch (dir)
        {
        case Room.DoorDirection.Left:
            return Left;
        case Room.DoorDirection.Right:
            return Right;
        case Room.DoorDirection.Down:
            return Down;
        case Room.DoorDirection.Up:
            return Up;
        }

        return null;
    }

    void Start()
    {
        Content = GetComponentInChildren<Image>();
    }

    public void Init(WorldData.RoomCell room)
    {
        Cell = room;
    }
}
