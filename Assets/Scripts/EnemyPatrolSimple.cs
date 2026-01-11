using UnityEngine;

public class EnemyPatrolSimple : MonoBehaviour
{
    public float patrolDistance = 5f; // Distance to patrol back and forth
    public float speed = 2f;         // Movement speed
    public float rotationSpeed = 10f; // Turning speed

    private CharacterController controller;
    private Vector3 startPosition;    // Initial position
    private float targetX;            // Target X position
    private bool movingRight = true;  // Direction flag

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startPosition = transform.position; // Store initial position
        targetX = startPosition.x + patrolDistance; // Start moving right
    }

    void Update()
    {
        // Calculate direction to target
        float currentX = transform.position.x;
        Vector3 direction = (new Vector3(targetX, transform.position.y, transform.position.z) - transform.position).normalized;

        // Rotate to face the direction
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move using Character Controller
        Vector3 move = direction * speed * Time.deltaTime;
        controller.Move(move);

        // Check if reached target
        if (Mathf.Abs(currentX - targetX) < 0.1f)
        {
            movingRight = !movingRight; // Switch direction
            targetX = movingRight ? (startPosition.x + patrolDistance) : (startPosition.x - patrolDistance);
        }
    }
}