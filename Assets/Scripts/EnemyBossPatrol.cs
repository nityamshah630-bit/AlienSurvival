using UnityEngine;

public class EnemyBossPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolDistance = 5f;
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            if (Vector3.Distance(startPosition, transform.position) >= patrolDistance)
                movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            if (Vector3.Distance(startPosition, transform.position) >= patrolDistance)
                movingRight = true;
        }
    }
}