using UnityEngine;
using System.Collections.Generic;

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

        distance = transform.position - Target.position;

        bool PlayerDistance = Mathf.Abs(distance.x) > MaxDistance.x
                              || distance.z > MaxDistance.z
                              || Mathf.Abs(distance.x) < MinDistance.x
                              || distance.z < MinDistance.z;

        if (PlayerDistance)
        {
            var pos = Target.position + MinDistance;
            TargetPos = new Vector3(pos.x, transform.position.y, pos.z);

            TargetPos.x = Mathf.Max(TargetPos.x, RoomCenter.x - (RoomSize.x * 0.5f));
            TargetPos.x = Mathf.Min(TargetPos.x, RoomCenter.x + (RoomSize.x * 0.5f));
            TargetPos.z = Mathf.Max(TargetPos.z, RoomCenter.z - (RoomSize.z * 0.5f));
            TargetPos.z = Mathf.Min(TargetPos.z, RoomCenter.z + (RoomSize.z * 0.5f));
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
