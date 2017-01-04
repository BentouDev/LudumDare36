using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;

public class MiniMapController : MonoBehaviour
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

    void HandleIcon(CellIcon icon)
    {
        var isDiscovered = icon.Cell.Reference.IsDiscovered;
        if (DisplayCoordinates)
        {
            var map = icon.Cell.Reference.MapPosition;
            icon.Text.text = map.x + ":" + map.y;
        }
        else
        {
            icon.Text.text = string.Empty;
        }

        if (!DisplayUndiscovered && !isDiscovered)
        {
            icon.SetVisible(false);
            return;
        }
        
        icon.SetVisible(true);

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

        icon.Background.sprite = NormalSprite;

        if (DisplayKeys && icon.Cell.Reference.KeyPickup != null)
        {
            icon.Content.sprite = KeySprite;
            icon.Content.color = Color.white;
        }
        else
        {
            icon.Content.sprite = null;
            icon.Content.color = Color.clear;
        }
        
        if (icon.Cell.Reference == game.World.GetCurrentRoom())
        {
            icon.Background.color = CurrentColor;
        }
        else
        {
            if (icon.Cell.Type == Room.RoomType.Boss)
            {
                icon.Background.color = Color.red;
            }
            else
            {
                icon.Background.color = UndiscoveredColor;
            }
        }
    }

    void Update()
    {
        foreach (var cell in CellIcons)
        {
            HandleIcon(cell);
        }
    }
}
