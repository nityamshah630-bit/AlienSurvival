using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float patrolDistance = 5f;
    private Vector3 startPos;
    private bool movingRight = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (transform.position.x > startPos.x + patrolDistance) movingRight = false;
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x < startPos.x - patrolDistance) movingRight = true;
        }
    }
}