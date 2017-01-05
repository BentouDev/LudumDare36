using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Positioning")]
    public Transform Target;
    public float Speed = 2;

    public Vector3 OffsetFromTarget;

    [Header("Minimal room size")]
    public float MinRoomSizeX = 6.5f;
    public float MinRoomSizeZ = 6.5f;

    [Header("Minimal margin from borders")]
    public float HorizontalMargin = 3;
    public float VerticalMargin = 3;

    private Vector3 RoomCenter;
    private Vector3 RoomSize;

    private Vector3 DistanceToTarget;
    private Vector3 TargetPos;

    public float MaxHorizontalOffset
    {
        get { return (0.5f * RoomSize.x) - HorizontalMargin; }
    }

    public float MaxVerticalOffset
    {
        get { return (0.5f * RoomSize.z) - VerticalMargin; }
    }
    
    public void Reset()
    {
        if (Target)
        {
            TargetPos.y = Target.position.y + OffsetFromTarget.y;

            if (RoomSize.x <= MinRoomSizeX)
            {
                TargetPos.x = RoomCenter.x + OffsetFromTarget.x;
            }
            else
            {
                TargetPos.x = Target.position.x + OffsetFromTarget.x;
            }

            if (RoomSize.z <= MinRoomSizeZ)
            {
                TargetPos.z = RoomCenter.z + OffsetFromTarget.z;
            }
            else
            {
                TargetPos.z = Target.position.z + OffsetFromTarget.z;
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

    private float GetTargetPosX(Vector3 targetOffset)
    {
        var targetOffsetX = Mathf.Abs(targetOffset.x);

        // If room is too small, center camera
        if (RoomSize.x <= MinRoomSizeX)
        {
            return RoomCenter.x + OffsetFromTarget.x;
        }
        // Move camera only if player is far enough from room border
        else if (targetOffsetX <= MaxHorizontalOffset)
        {
            return Target.position.x + OffsetFromTarget.x;
        }

        return TargetPos.x;
    }

    private float GetTargetPosZ(Vector3 targetOffset)
    {
        var targetOffsetZ = Mathf.Abs(targetOffset.z);

        // If room is too small, center camera
        if (RoomSize.z <= MinRoomSizeZ)
        {
            return RoomCenter.z + OffsetFromTarget.z;
        }
        // Move camera only if player is far enough from room border
        else if (targetOffsetZ <= MaxVerticalOffset)
        {
            return Target.position.z + OffsetFromTarget.z;
        }

        return TargetPos.z;
    }

    void FixedUpdate()
    {
        if (!Target)
            return;

        var targetOffsetFromCenter = Target.position - RoomCenter;

        TargetPos.y = Target.position.y + OffsetFromTarget.y;
        TargetPos.x = GetTargetPosX(targetOffsetFromCenter);
        TargetPos.z = GetTargetPosZ(targetOffsetFromCenter);

        DistanceToTarget = transform.position - Target.position;

        transform.position = Vector3.Lerp (
            transform.position, TargetPos, 
            Time.fixedDeltaTime * Speed * DistanceToTarget.normalized.magnitude
        );
    }
}
