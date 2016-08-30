using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public string OpenAnim;
    public string OpenAnimIdle;
    public string ClodeAnimIdle;

    public Animator Anim;
    
    public Key KeyInstance { get; protected set; }

    public MeshRenderer Renderer;
    public int MaterialInstance;

    public WorldController World;
    public Room.DoorDirection Direction;

    private bool isLocked;

    [System.Serializable]
    public class Key
    {
        [SerializeField]
        public Color color;
    }
    
    public void Unlock()
    {
        if (KeyInstance != null)
            return;

        Anim.Play(OpenAnim);
        isLocked = false;
    }

    public void Lock()
    {
        if (KeyInstance != null)
            return;

        Anim.Play(ClodeAnimIdle);
        isLocked = true;
    }

    public void UnlockWithKey(Key key)
    {
        if (this.KeyInstance == key)
        {
            KeyInstance = null;
            Unlock();
        }
    }

    public void Init(WorldController world, Room.DoorDirection left, Key keyInstance)
    {
        if(!Anim)
            Anim = GetComponent<Animator>();

        KeyInstance = keyInstance;
        World = world;
        Direction = left;

        if (KeyInstance != null && Renderer && MaterialInstance < Renderer.materials.Length)
        {
            Renderer.materials[MaterialInstance].color = KeyInstance.color;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        var pawn = collider.gameObject.GetComponentInParent<Pawn>();
        if (pawn && World)
        {
            if (isLocked || KeyInstance != null)
                return;

            World.OnEnterDoor(Direction);
        }
    }
}
