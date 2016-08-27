using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapController : MonoBehaviour
{
    public Color CurrentColor;
    public Color UndiscoveredColor;

    public GameObject EmptyCell;
    public GameObject UndiscoveredCell;
    public GridLayoutGroup Grid;

    private WorldData data;
    private Game game;

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
                    var textComponent = go.GetComponentInChildren<Text>();
                        textComponent.text = "" + i + "::" + j;

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
            if (cell.Cell.Reference == game.World.GetCurrentRoom())
            {
                cell.Image.color = CurrentColor;
            }
            else
            {
                cell.Image.color = UndiscoveredColor;
            }
        }
    }
}
