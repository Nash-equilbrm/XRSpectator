using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlayer : MonoBehaviour
{
    public Transform follow;
    public Vector3 offset;
    private void Update()
    {
        transform.position = follow.position + offset;
        transform.rotation = follow.rotation;

    }
}
