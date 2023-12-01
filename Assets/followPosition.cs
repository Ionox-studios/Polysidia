using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPosition : MonoBehaviour
{
    public Transform target; // Assign the target transform (e.g., the lid of the bottle) in the Inspector
    private Quaternion worldRotation;

    void Start()
    {
        // Store the world rotation of the particle system at start
        worldRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Update position to match the target's position
            transform.position = target.position;

            // Maintain the initial world rotation
            transform.rotation = worldRotation;
        }
    }
}

