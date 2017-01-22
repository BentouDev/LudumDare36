using UnityEngine;
using System.Collections.Generic;

public abstract class GenericPickup : MonoBehaviour
{
    public bool RemoveOnNextLevel;
    public bool Consumeable;

    public Sprite MiniMapIcon;

    protected Room ParentRoom;

    public void Init(Room parentRoom)
    {
        ParentRoom = parentRoom;

        OnInit();
    }

    protected virtual void OnInit()
    { }

    protected abstract void OnPickup(Pawn pawn);

    void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.GetComponentInParent<Pawn>();
        if (pawn)
        {
            if (ParentRoom)
            {
                ParentRoom.OnPickup(pawn);
            }

            if (!Consumeable)
            {
                pawn.AddPickup(this);
            }

            OnPickup(pawn);

            gameObject.SetActive(false);
        }
    }
}
