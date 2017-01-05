using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldController : MonoBehaviour
{
    [System.Serializable]
    public struct PickupInfo
    {
        [SerializeField]
        public GameObject PickupPrefab;

        [SerializeField]
        public bool Singleton;

        [SerializeField]
        public int Chance;

        [HideInInspector]
        public int SpawnCount;
    }

    [Header("Pickups")]
    public GameObject KeyPickupPrefab;
    public GameObject DeepDoorPrefab;

    [Range(0.1f,1)]
    public float PickupSpawnChance = 0.75f;
    public List<PickupInfo> OtherPickups;

    [Header("Doors")]
    public GameObject DoorPrefab;
    public GameObject BossDoorPrefab;

    [Header("Enemies")]
    public GameObject EnemyPrefab;

    private readonly List<Door> CurrentDoors = new List<Door>();

    private Room CurrentRoom;
    private Game Game;
    private WorldData Data;

    private bool WasCleared;
    
    public int PickupChanceCount
    {
        get { return OtherPickups.Sum(o => o.Chance); }
    }
    
    public Room GetCurrentRoom()
    {
        return CurrentRoom;
    }

    public void CreateWorld(Game game, WorldData data)
    {
        Game = game;
        Data = data;
        ActivateRoom(Data.FirstRoom, Vector3.zero);

        for (int i = 0; i < OtherPickups.Count; i++)
        {
            var pickup = OtherPickups[i];
                pickup.SpawnCount = 0;
            OtherPickups[i] = pickup;
        }
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

            TryToSpawnPickup();
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
                TryToSpawnPickup();
            }
        }
        
        UnlockDoors();
    }

    private void TryToSpawnPickup()
    {
        float spawnChance = Random.Range(0.0f, 1.0f);
        if (spawnChance > (1 - PickupSpawnChance))
        {
            SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        var prefab = TryGetRandomPickup();
        if (prefab != null)
        {
            var go = Instantiate(prefab, CurrentRoom.transform) as GameObject;
                go.transform.position = CurrentRoom.transform.position;

            Game.Music.PlayMystery();
        }
    }

    private GameObject TryGetRandomPickup()
    {
        float random = Random.Range(0.0f, 1.0f);
        
        var pickup = new PickupInfo();
        var chanceAccumulator = 0;
        var availablePickups = OtherPickups.Where(o => (!o.Singleton || o.SpawnCount == 0)).ToList();

        foreach (var currentPickup in availablePickups)
        {
            chanceAccumulator += currentPickup.Chance;

            if (chanceAccumulator/(float) PickupChanceCount > random)
            {
                pickup = currentPickup;
                break;
            }
        }
        
        if (pickup.Singleton && pickup.SpawnCount > 0)
            return null;

        if (pickup.PickupPrefab != null)
        {
            var index = OtherPickups.IndexOf(pickup);
            pickup.SpawnCount++;
            OtherPickups[index] = pickup;
            
            return pickup.PickupPrefab;
        }

        return null;
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
