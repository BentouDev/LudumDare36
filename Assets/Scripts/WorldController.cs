using System;
using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public GameObject DoorPrefab;

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
        CurrentRoom = room;
        BuildDoors(room);

        Game.Player.SetupPawn(position);
    }

    public void BuildDoors(Room room)
    {
        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            if(!room.HasRoom(Data, dir))
                continue;
            
            var go = Instantiate(DoorPrefab, room.transform) as GameObject;
            var door = go.GetComponent<Door>();

            door.Init(this, dir);
            door.transform.position = GetDoorOffset(room, dir);
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
        var inverse = Room.InverseDirection[direction];
        var room = CurrentRoom.GetRoomCell(Data, direction);
        ActivateRoom(room.Reference, GetDoorOffset(room.Reference, inverse, 0.25f));
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
