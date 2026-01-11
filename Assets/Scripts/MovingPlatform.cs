using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.up;
    public float distance = 3f;
    public float speed = 1f;
    private Vector3 startPos;
    private bool movingUp = true;

    void Start() { startPos = transform.position; }

    void Update()
    {
        float step = speed * Time.deltaTime;
        if (movingUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos + moveDirection * distance, step);
            if (Vector3.Distance(transform.position, startPos + moveDirection * distance) < 0.1f) movingUp = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, step);
            if (Vector3.Distance(transform.position, startPos) < 0.1f) movingUp = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.parent = transform;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.parent = null;
    }
}