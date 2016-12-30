using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    [Header("Pickups")]
    public GameObject KeyPickupPrefab;
    public GameObject DeepDoorPrefab;

    public List<GameObject> HealthPrefab;

    [Header("Doors")]
    public GameObject DoorPrefab;
    public GameObject BossDoorPrefab;

    [Header("Enemies")]
    public GameObject EnemyPrefab;

    private Game Game;
    private WorldData Data;

    private Room CurrentRoom;
    private List<Door> CurrentDoors = new List<Door>();

    private bool WasCleared;

    private bool hasWayToBoss;
    private bool hasWayFromBoss;

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
            foreach (Door door in CurrentDoors)
            {
                DestroyObject(door.gameObject);
            }

            CurrentDoors.Clear();

            CurrentRoom.gameObject.SetActive(false);

            var bullets = FindObjectsOfType<Bullet>();
            foreach (Bullet bullet in bullets)
            {
                DestroyObject(bullet.gameObject);
            }
        }

        room.gameObject.SetActive(true);
        room.ShowAllDoorPlaceholders();
        
        SpawnEnemies(room);

        CurrentRoom = room;

        BuildDoors(room);

        var isCleared = room.IsCleared;

        if (isCleared)
        {
            if (!room.IsDiscovered && room.KeyPickup != null)
            {
                OnRoomCleared();
            }
            else
            {
                UnlockDoors();
            }
        }

        WasCleared = isCleared;

        room.IsDiscovered = true;

        Game.Player.SetupPawn(position);
        Game.Camera.SetRoomBounds(CurrentRoom.transform.position, CurrentRoom.RoomSize);
        Game.Camera.Reset();
    }

    void Update()
    {
        if (!CurrentRoom)
            return;

        if (!WasCleared && CurrentRoom.IsCleared)
        {
            OnRoomCleared();
        }

        WasCleared = CurrentRoom.IsCleared;
    }

    private void OnRoomCleared()
    {
        bool isBoss = Data.RoomMap[CurrentRoom.MapPosition.x][CurrentRoom.MapPosition.y].Type == Room.RoomType.Boss;
        if (isBoss)
        {
            Game.Music.PlayMystery();

            var deep = Instantiate(DeepDoorPrefab, CurrentRoom.transform) as GameObject;
                deep.transform.position = CurrentRoom.transform.position;

            var deepDoor = deep.GetComponentInChildren<DeepDoors>();
                deepDoor.Init(Game);

            float random = Random.Range(0.0f, 1.0f);
            if (random > 0.5f)
            {
                Game.Music.PlayMystery();

                int index = Random.Range(0, HealthPrefab.Count);
                var go = Instantiate(HealthPrefab[index], CurrentRoom.transform) as GameObject;
                go.transform.position = CurrentRoom.transform.position;
            }
        }
        else
        {
            if (CurrentRoom.KeyPickup != null)
            {
                Game.Music.PlayMystery();

                var go = Instantiate(KeyPickupPrefab, CurrentRoom.transform) as GameObject;
                go.transform.position = CurrentRoom.transform.position;

                var key = go.GetComponent<KeyPickup>();
                key.Init(CurrentRoom.KeyPickup);
            }
            else
            {
                float random = Random.Range(0.0f, 1.0f);
                if (random > 0.5f)
                {
                    Game.Music.PlayMystery();

                    int index = Random.Range(0, HealthPrefab.Count);
                    var go = Instantiate(HealthPrefab[index], CurrentRoom.transform) as GameObject;
                    go.transform.position = CurrentRoom.transform.position;
                }
            }
        }
        
        UnlockDoors();
    }

    private void UnlockDoors()
    {
        foreach (Door door in CurrentDoors)
        {
            door.Unlock();
        }
    }

    private void SpawnEnemies(Room room)
    {
        var enemies = GameObject.FindGameObjectsWithTag("EnemySpawn");
        foreach (var enemy in enemies)
        {
            var go = Instantiate(EnemyPrefab, room.transform) as GameObject;
                go.transform.position = enemy.transform.position;

            DestroyObject(enemy.gameObject);
        }

        room.AwakeEnemies(Game.Player.Pawn);
    }

    public void BuildDoors(Room room)
    {
        bool isBoss = Data.RoomMap[room.MapPosition.x][room.MapPosition.y].Type == Room.RoomType.Boss;

        foreach (Room.DoorDirection dir in Room.AllDirs)
        {
            /*var cell = room.GetGlobalRoomCell(Data, dir);
            if(cell.Type == Room.RoomType.Empty)
                continue;

            bool toBoss = cell.Type == Room.RoomType.Boss;
            bool isFirstRoom = room == Data.FirstRoom || cell.Reference == Data.FirstRoom;

            if (((isBoss || toBoss) && isFirstRoom) || (isBoss && hasWayToBoss) || (toBoss && hasWayToBoss))
                continue;

            room.HideDoorPlaceholder(dir);

            hasWayToBoss |= isBoss;
            hasWayFromBoss |= toBoss;*/

            var cell = room.GetConnectedRoomCell(dir);
            if (cell.Type == Room.RoomType.Empty)
                continue;

            room.HideDoorPlaceholder(dir);
            
            var go = Instantiate(isBoss ? BossDoorPrefab : DoorPrefab, room.transform) as GameObject;
            var door = go.GetComponent<Door>();
            var doorPos = room.GetDoorPosition(dir);

            if (doorPos.magnitude < 0.1f)
            {
                doorPos = GetDoorOffset(room, dir);
            }

            door.Init(this, dir, cell.Reference.KeyLock);
            door.transform.position = doorPos;
            door.transform.forward = Room.DoorForward[dir];

            CurrentDoors.Add(door);
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

        CurrentRoom.KeyLock = null;

        var inverse = Room.InverseDirection[direction];
        var room = CurrentRoom.GetGlobalRoomCell(Data, direction);

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
            WorldData.RoomCell cell = room.GetGlobalRoomCell(Data, dir);
            if (cell.Type != Room.RoomType.Empty)
            {
                result.Add(cell.Reference);
            }
        }

        return result;
    }
}
