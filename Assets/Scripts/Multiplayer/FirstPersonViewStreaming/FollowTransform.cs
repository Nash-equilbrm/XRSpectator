using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{

    [Header("Position")]
    public Transform follow;
    public Vector3 offset;
    
    public bool lockX = false;
    public float lockXAt;
    
    public bool lockY = false;
    public float lockYAt;

    public bool lockZ = false;
    public float lockZAt;

    [Header("Rotation")]
    public bool copyRotation = true;
    public bool faceTransform = false;

    private void Update()
    {
        if (follow)
        {
            Vector3 position = follow.position + offset;
            if (lockX)
            {
                position.x = lockXAt;
            }
            if (lockY)
            {
                position.y = lockYAt;
            }
            if (lockZ)
            {
                position.x = lockZAt;
            }

            transform.position = position;


            if (copyRotation)
            {
                transform.rotation = follow.rotation;
            }
            if (faceTransform)
            {
                transform.LookAt(follow);
            }
        }

    }
}
