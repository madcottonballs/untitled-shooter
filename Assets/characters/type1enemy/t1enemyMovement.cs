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
    [SerializeField] int health; // meant to be dependent on the scene

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (health <= 0) // kills the sprite
        {
            Destroy(gameObject);
        }

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
        if (other.CompareTag("Player"))
        {
            awareOfPlayer = true; // if they're touched by the player, they see him
            if (Player.isLunging) // only take damage during the lunge
            {
                health -= Player.lungeDamage;
            }
        }
    }

}
