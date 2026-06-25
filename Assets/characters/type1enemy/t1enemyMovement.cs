using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class t1enemyMovement : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public attack Player;
    public bool awareOfPlayer = false;

    Rigidbody2D rb;
    int index;
    int patrolDirection = 1;
    [SerializeField] int health; // meant to be dependent on the scene
    Transform player;
    float lastPlayerDistance;

    void Awake()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");  
        player = temp.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (health <= 0) // kills the sprite
        {
            Destroy(gameObject);
        }

        if (points == null || points.Length == 0) // if there's no points stop patrol
        {
            return;
        }
        if (awareOfPlayer) // if aware of player, stop patrol and stare at player
        {
            if (player != null)
            {
                // Measure whether the player is moving away from us this frame.
                float currentPlayerDistance = Vector2.Distance(rb.position, player.position);

                // Keep the enemy facing the player every physics frame.
                Vector2 direction = (Vector2)player.position - rb.position;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    transform.right = direction.normalized;
                }

                // Only follow if the player is increasing the distance between us.
                if (currentPlayerDistance > lastPlayerDistance)
                {
                    rb.MovePosition(Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime));
                }

                lastPlayerDistance = currentPlayerDistance;
            }
            return;
        }

        Transform target = points[index];
        Vector2 movementDirection = ((Vector2)target.position - rb.position).normalized;

        // Move one physics step toward the current waypoint.
        rb.MovePosition(Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime));

        // Face the direction of travel so the enemy visually turns while patrolling.
        if (movementDirection.sqrMagnitude > 0.0001f)
        {
            transform.right = movementDirection;
        }

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

    public void Alert()
    {
        awareOfPlayer = true;
        if (player != null)
        {
            lastPlayerDistance = Vector2.Distance(rb.position, player.position);
        }
    }

}
