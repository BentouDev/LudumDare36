using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CellIcon : MonoBehaviour
{
    public Image Image;
    public WorldData.RoomCell Cell;

    void Start()
    {
        Image = GetComponentInChildren<Image>();
    }

    public void Init(WorldData.RoomCell room)
    {
        Cell = room;
    }
}
