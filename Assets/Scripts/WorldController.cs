using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public GameObject DoorPrefab;
    public GameObject BossDoorPrefab;

    private Game Game;
    private WorldData Data;

    private Room CurrentRoom;
    private List<Transform> toDestroy = new List<Transform>();

    public Room GetCurrentRoom()
    {
        return CurrentRoom;
    }

    public void CreateWorld(Game game, WorldData data)
    {
        Game = game;
        Data = data;
        ActivateRoom(Data.FirstRoom, Vector3.zero);
    }

    public void ActivateRoom(Room room, Vector3 position)
    {
        if (CurrentRoom)
        {
            foreach (Transform tr in toDestroy)
            {
                DestroyObject(tr.gameObject);
            }

            toDestroy.Clear();

            CurrentRoom.gameObject.SetActive(false);
        }

        room.gameObject.SetActive(true);
        room.ShowAllDoorPlaceholders();
        room.AwakeEnemies(Game.Player.Pawn);

        CurrentRoom = room;

        BuildDoors(room);

        Game.Camera.SetRoomBounds(CurrentRoom.transform.position, CurrentRoom.RoomSize);
        Game.Player.SetupPawn(position);
    }

    public void BuildDoors(Room room)
    {
        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            var cell = room.GetRoomCell(Data, dir);
            if(cell.Type == Room.RoomType.Empty)
                continue;

            room.HideDoorPlaceholder(dir);

            bool isBoss = cell.Type == Room.RoomType.Boss ||
                Data.RoomMap[room.MapPosition.x][room.MapPosition.y].Type == Room.RoomType.Boss;
            
            var go = Instantiate(isBoss ? BossDoorPrefab : DoorPrefab, room.transform) as GameObject;
            var door = go.GetComponent<Door>();
            var doorPos = room.GetDoorPosition(dir);

            if (doorPos.magnitude < 0.1f)
            {
                doorPos = GetDoorOffset(room, dir);
            }

            door.Init(this, dir);
            door.transform.position = doorPos;
            door.transform.forward = Room.DoorForward[dir];

            toDestroy.Add(door.transform);
        }
    }

    public Vector3 GetDoorOffset(Room room, Room.DoorDirection direction, float distance = 0.5f)
    {
        Vector3 result = Vector3.zero;

        switch (direction)
        {
            case Room.DoorDirection.Left:
                result = new Vector3(-distance * room.RoomSize.x, 0, 0);
                break;
            case Room.DoorDirection.Right:
                result = new Vector3(distance * room.RoomSize.x, 0, 0);
                break;
            case Room.DoorDirection.Down:
                result = new Vector3(0, 0, -distance * room.RoomSize.z);
                break;
            case Room.DoorDirection.Up:
                result = new Vector3(0, 0, distance * room.RoomSize.z);
                break;
        }

        return result;
    }

    public void OnEnterDoor(Room.DoorDirection direction)
    {
        if (!CurrentRoom.IsCleared)
            return;

        var inverse = Room.InverseDirection[direction];
        var room = CurrentRoom.GetRoomCell(Data, direction);

        var doorPos = room.Reference.GetDoorPosition(inverse);
        if (doorPos.magnitude < 0.1f)
        {
            doorPos = GetDoorOffset(room.Reference, inverse);
        }

        doorPos -= Room.DoorForward[inverse];
        doorPos.y = 0;

        ActivateRoom(room.Reference, doorPos);
    }

    public IList<Room> GetAdjacent(Room room)
    {
        IList<Room> result = new List<Room>();

        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            WorldData.RoomCell cell = room.GetRoomCell(Data, dir);
            if (cell.Type != Room.RoomType.Empty)
            {
                result.Add(cell.Reference);
            }
        }

        return result;
    }
}
