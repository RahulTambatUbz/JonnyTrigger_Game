using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset;   // Offset from the player's position

    void Start()
    {
        // You can set the offset directly in the inspector, or calculate it here
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Update the camera's position based on the player's position plus the offset
            transform.position = player.position + offset;
        }
    }
}
