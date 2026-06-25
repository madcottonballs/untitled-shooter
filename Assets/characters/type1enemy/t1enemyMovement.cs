using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class t1enemyMovement : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public attack Player;

    Rigidbody2D rb;
    int index;
    int patrolDirection = 1;
    bool awareOfPlayer = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (points == null || points.Length == 0 || awareOfPlayer) // if there's no points or aware of player, stop patrol
        {
            return;
        }

        Transform target = points[index];

        // Move one physics step toward the current waypoint.
        rb.MovePosition(Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime));

        // Once we get close enough, switch to the next waypoint.
        if (Vector2.Distance(rb.position, target.position) < 0.1f)
        {
            if (index == points.Length - 1)
            {
                patrolDirection = -1;
            }
            else if (index == 0)
            {
                patrolDirection = 1;
            }

            index += patrolDirection;
            index = Mathf.Clamp(index, 0, points.Length - 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) // handles collision
    {
        if (!Player.isLunging && other.CompareTag("Player")) // detects collision and checks it is the player and they are lunging
        {
            awareOfPlayer = true; // if they're hurt by the player, they see him

            
        }
    }

}
