using UnityEngine;
using System.Collections.Generic;

public class FollowCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 MaxDistance;
    public Vector3 MinDistance;

    private Vector3 RoomCenter;
    private Vector3 RoomSize;

    private Vector3 distance;

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
            var targetPos = new Vector3(pos.x, transform.position.y, pos.z);

            targetPos.x = Mathf.Max(targetPos.x, RoomCenter.x - (RoomSize.x * 0.5f));
            targetPos.x = Mathf.Min(targetPos.x, RoomCenter.x + (RoomSize.x * 0.5f));
            targetPos.z = Mathf.Max(targetPos.z, RoomCenter.z - (RoomSize.z * 0.5f));
            targetPos.z = Mathf.Min(targetPos.z, RoomCenter.z + (RoomSize.z * 0.5f));

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime);

        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 200, 10, 200, 30), "Distance " + distance);
    }
}
