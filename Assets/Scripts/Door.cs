using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public WorldController World;
    public Room.DoorDirection Direction;

    public void Init(WorldController world, Room.DoorDirection left)
    {
        World = world;
        Direction = left;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (World)
        {
            World.OnEnterDoor(Direction);
        }
    }
}
