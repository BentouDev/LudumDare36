using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;

public class MiniMapController : MonoBehaviour, ILevelDependable
{
    public Sprite KeySprite;
    public Sprite NormalSprite;

    public Color CurrentColor;
    public Color UndiscoveredColor;
    public Color MainColor;

    public GameObject EmptyCell;
    public GameObject UndiscoveredCell;
    public GameObject LockedCell;
    public GridLayoutGroup Grid;

    private WorldData data;
    private Game game;

    public bool DisplayCoordinates;
    public bool DisplayUndiscovered;
    public bool DisplayKeys;
    public bool DisplayLocks;

    private List<CellIcon> CellIcons = new List<CellIcon>();
    private List<GameObject> EmptyCells = new List<GameObject>();
    
    public void OnLevelLoaded()
    {

    }
    
    public void OnLevelCleanUp()
    {
        DisplayKeys = false;
        DisplayUndiscovered = false;

        foreach (CellIcon icon in CellIcons)
        {
            Destroy(icon.gameObject);
        }

        foreach (GameObject cell in EmptyCells)
        {
            Destroy(cell);
        }

        CellIcons.Clear();
        EmptyCells.Clear();
    }

    public void Init(Game game, WorldData data)
    {
        this.game = game;
        this.data = data;
        Grid.constraintCount = data.WorldWidth;

        for (int i = 0; i < data.WorldHeight; i++)
        {
            for (int j = 0; j < data.WorldWidth; j++)
            {
                var cell = data.RoomMap[j][i];
                if (cell.Type == Room.RoomType.Empty)
                {
                    var go = Instantiate(EmptyCell, Grid.transform) as GameObject;

                    EmptyCells.Add(go);
                }
                else
                {
                    GameObject go = Instantiate(UndiscoveredCell, Grid.transform) as GameObject;

                    var icon = go.GetComponentInChildren<CellIcon>();
                        icon.Cell = cell;

                    CellIcons.Add(icon);
                }
            }
        }
    }
    
    void Update()
    {
        foreach (var cell in CellIcons)
        {
            DisplayCell(cell);
        }
    }

    void SetCellIconContent(CellIcon icon, bool isDiscovered)
    {
        var room = icon.Cell.Reference;

        if (isDiscovered && room.ActivePickup != null)
        {
            icon.Content.sprite = room.ActivePickup.MiniMapIcon;
            icon.Content.color = Color.white;
        }
        else if (DisplayKeys && room.KeyPickup != null && !room.PickedUpKey)
        {
            icon.Content.sprite = KeySprite;
            icon.Content.color = Color.white;
        }
        else
        {
            icon.Content.sprite = null;
            icon.Content.color = Color.clear;
        }
    }

    string GetCellDescription(CellIcon icon)
    {
        if (DisplayCoordinates)
        {
            var map = icon.Cell.Reference.MapPosition;
            return map.x + ":" + map.y;
        }

        return string.Empty;
    }

    void DisplayCellDoors(CellIcon icon, bool isDiscovered)
    {
        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            var img = icon.GetImage(dir);
            var connection = icon.Cell.Reference.GetConnectedRoomCell(dir);
            if (connection.Type != Room.RoomType.Empty)
            {
                img.gameObject.SetActive(true);
                img.color = (DisplayLocks || isDiscovered) && connection.Reference.KeyLock != null ? connection.Reference.KeyLock.color : Color.white;
            }
            else
            {
                img.gameObject.SetActive(false);
            }
        }
    }

    Color GetCellColor(CellIcon icon)
    {
        if (icon.Cell.Reference == game.World.GetCurrentRoom())
        {
            return CurrentColor;
        }
        else
        {
            if (icon.Cell.Type == Room.RoomType.Boss)
            {
                return Color.red;
            }
            else
            {
                return UndiscoveredColor;
            }
        }
    }

    void DisplayCell(CellIcon icon)
    {

        icon.Text.text = GetCellDescription(icon);

        var isDiscovered = icon.Cell.Reference.IsDiscovered;
        SetCellIconContent(icon, isDiscovered);
        if (!DisplayUndiscovered && !isDiscovered)
        {
            icon.SetVisible(false);
            return;
        }
        
        icon.SetVisible(true);

        icon.Background.sprite = NormalSprite;
        icon.Background.color = GetCellColor(icon);

        DisplayCellDoors(icon, isDiscovered);
    }
}
