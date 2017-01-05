using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class FollowCamera : MonoBehaviour
{
    public bool drawDebug;
    public Transform Target;
    public float Speed = 2;

    private Vector3 RoomCenter;
    private Vector3 RoomSize;

    private Vector3 distance;
    
    private Vector3 TargetPos;

    public Vector3 CameraOffset;

    public float TargetRoomSizeX = 6.5f;
    public float TargetRoomSizeY = 6.5f;
    public float MaxHorizontalOffset;
    public float MaxVerticalOffset;
    
    public void Reset()
    {
        if (Target)
        {
            distance = transform.position - Target.position;
            TargetPos.y = Target.position.y + CameraOffset.y;

            if (RoomSize.x <= TargetRoomSizeX)
            {
                TargetPos.x = RoomCenter.x + CameraOffset.x;
            }
            else
            {
                TargetPos.x = Target.position.x + CameraOffset.x;
            }

            if (RoomSize.z <= TargetRoomSizeY)
            {
                TargetPos.z = RoomCenter.z + CameraOffset.z;
            }
            else
            {
                TargetPos.z = Target.position.z + CameraOffset.z;
            }

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
}
