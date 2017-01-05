using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class FollowCamera : MonoBehaviour
{
    public bool drawDebug;
    public Transform Target;
    public float Speed = 2;
    public Vector3 MaxDistance;
    public Vector3 MinDistance;

    private Vector3 RoomCenter;
    private Vector3 RoomSize;

    private Vector3 distance;

    private Vector3 StartPos;
    private Vector3 TargetPos;

    public Vector3 CameraOffset;

    public float TargetRoomSizeX = 6.5f;
    public float TargetRoomSizeY = 6.5f;
    public float MaxHorizontalOffset;
    public float MaxVerticalOffset;

    void Start()
    {
        StartPos = transform.position;
    }

    public void Reset()
    {
        if (Target)
        {
            var pos = Target.position + MinDistance;

            TargetPos = new Vector3(pos.x, transform.position.y, pos.z);
            TargetPos.x = Mathf.Max(TargetPos.x, RoomCenter.x - (RoomSize.x * 0.5f));
            TargetPos.x = Mathf.Min(TargetPos.x, RoomCenter.x + (RoomSize.x * 0.5f));
            TargetPos.z = Mathf.Max(TargetPos.z, RoomCenter.z - (RoomSize.z * 0.5f));
            TargetPos.z = Mathf.Min(TargetPos.z, RoomCenter.z + (RoomSize.z * 0.5f));

            transform.position = TargetPos;
        }
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    public void SetRoomBounds(Vector3 pos, Vector3 size)
    {
        RoomCenter = pos;
        RoomSize = size;
    }

    void FixedUpdate()
    {
        if (!Target)
            return;

        /*

        bool PlayerDistance = Mathf.Abs(distance.x) > MaxDistance.x
                              || distance.z > MaxDistance.z
                              || Mathf.Abs(distance.x) < MinDistance.x
                              || distance.z < MinDistance.z;

        if (PlayerDistance)
        {
            var pos = Target.position + MinDistance;
            TargetPos = new Vector3(pos.x, transform.position.y, pos.z);

            /*TargetPos.x = Mathf.Max(TargetPos.x, RoomCenter.x - (RoomSize.x * 0.5f));
            TargetPos.x = Mathf.Min(TargetPos.x, RoomCenter.x + (RoomSize.x * 0.5f));
            TargetPos.z = Mathf.Max(TargetPos.z, RoomCenter.z - (RoomSize.z * 0.5f));
            TargetPos.z = Mathf.Min(TargetPos.z, RoomCenter.z + (RoomSize.z * 0.5f));//
        }*/

        distance = transform.position - Target.position;
        var offset = Target.position - RoomCenter;

        TargetPos.y = Target.position.y + CameraOffset.y;

        if (RoomSize.x <= TargetRoomSizeX)
        {
            TargetPos.x = RoomCenter.x + CameraOffset.x;
        }
        else if (!(Mathf.Abs(offset.x) > (0.5f * RoomSize.x) - MaxHorizontalOffset))
        {
            TargetPos.x = Target.position.x + CameraOffset.x;
        }

        if (RoomSize.z <= TargetRoomSizeY)
        {
            TargetPos.z = RoomCenter.z + CameraOffset.z;
        }
        else if (!(Mathf.Abs(offset.z) > (0.5f*RoomSize.z) - MaxVerticalOffset))
        {
            TargetPos.z = Target.position.z + CameraOffset.z;
        }
        
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.fixedDeltaTime * Speed * distance.normalized.magnitude);
    }

    void OnGUI()
    {
        if (!drawDebug)
            return;

        GUI.Label(new Rect(Screen.width - 200, 10, 200, 30), "dst : " + distance);
    }
}
