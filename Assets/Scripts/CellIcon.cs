using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class CellIcon : MonoBehaviour
{
    public Image Content;
    public Image Left;
    public Image Right;
    public Image Down;
    public Image Up;
    public CanvasGroup Group;

    public WorldData.RoomCell Cell;

    void Start()
    {
        if(!Group)
            Group = GetComponent<CanvasGroup>();

        if(!Content)
            Content = GetComponentInChildren<Image>();
    }

    public void SetVisible(bool value)
    {
        if (value)
            Group.alpha = 1;
        else
        {
            Group.alpha = 0;
        }
    }

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

    public void Init(WorldData.RoomCell room)
    {
        Cell = room;
    }
}
