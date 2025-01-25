using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Tooltip("The target this GameObject will follow")]
    public Transform target;

    [Tooltip("The fixed offset above the target")]
    public Vector3 offset = new Vector3(0, 2, 0);

    void LateUpdate()
    {
        if (target != null)
        {
            // Set the position of this GameObject to the target's position plus the offset
            transform.position = target.position + offset;
        }
    }
}
