using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClampScript : MonoBehaviour
{
    public Transform target; // The target to follow (e.g., player)
    public Vector2 minClamp = new Vector2(-5f, -5f); // Minimum allowed position
    public Vector2 maxClamp = new Vector2(5f, 5f);   // Maximum allowed position

    void LateUpdate()
    {
        if (target != null)
        {
            // Clamp the camera position based on the target's position
            float clampedX = Mathf.Clamp(target.position.x, minClamp.x, maxClamp.x);
            float clampedY = Mathf.Clamp(target.position.y, minClamp.y, maxClamp.y);

            // Set the camera's position to the clamped values
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
