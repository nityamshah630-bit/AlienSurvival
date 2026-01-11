using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player, set in Inspector
    public Vector3 offset;  // Offset from the target

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}