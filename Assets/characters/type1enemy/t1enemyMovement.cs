using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class t1enemyMovement : MonoBehaviour
{
    public Transform[] points; // Patrol waypoints.
    public float speed = 2f; // Patrol and chase movement speed.
    public attack Player; // Reference to the player's attack script.
    public bool awareOfPlayer = false; // True once the enemy has noticed the player.

    Rigidbody2D rb; // Cached Rigidbody2D for movement.
    int index; // Current patrol waypoint index.
    int patrolDirection = 1; // Patrol direction, 1 forward and -1 backward.
    [SerializeField] public int health; // Enemy health for this scene.
    [SerializeField] float fleeThreshold = 0.1f; // How strongly the player must move away to trigger chase.
    [SerializeField] float distanceThreshold = 0.02f; // Small buffer to ignore tiny distance changes.
    [SerializeField] float linearDrag = 8f; // Drag used to slow knockback from collisions.
    movement playerMovement; // Cached player movement script.
    Transform player; // Cached player transform.
    bool isFollowingPlayer; // True while the enemy is actively chasing.
    float lastPlayerDistance; // Player distance from the previous physics frame.
    public bool playerInVision; // True while the player is inside the vision cone.

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = linearDrag;
        rb.angularDrag = 0f;
        rb.freezeRotation = true;

        GameObject temp = FindPlayerObject();
        if (temp != null)
        {
            player = temp.transform;
            playerMovement = temp.GetComponent<movement>();
        }
    }

    void FixedUpdate()
    {
        if (health <= 0) // kills the sprite
        {
            Destroy(gameObject);
        }

        if (awareOfPlayer) // if aware of player, stop patrol and stare at player
        {
            if (player != null)
            {
                // Keep the enemy facing the player every physics frame.
                Vector2 toPlayer = (Vector2)player.position - rb.position;
                float currentPlayerDistance = toPlayer.magnitude;
                if (toPlayer.sqrMagnitude > 0.0001f)
                {
                    transform.right = toPlayer.normalized;
                }

                // Only enter the follow state if the player is visible and clearly moving away.
                if (playerInVision && playerMovement != null)
                {
                    float playerMovesAwayAmount = Vector2.Dot(playerMovement.velocity, toPlayer.normalized);
                    if (playerMovesAwayAmount > fleeThreshold)
                    {
                        isFollowingPlayer = true;
                    }
                    else if (playerMovesAwayAmount < -fleeThreshold || currentPlayerDistance < lastPlayerDistance - distanceThreshold)
                    {
                        isFollowingPlayer = false;
                    }
                }

                if (isFollowingPlayer)
                {
                    rb.MovePosition(Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime));
                }
                else if (!playerInVision)
                {
                    isFollowingPlayer = false;
                }

                lastPlayerDistance = currentPlayerDistance;
            }
            return;
        }

        if (points == null || points.Length == 0) // if there's no points stop patrol
        {
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
        isFollowingPlayer = false;
        if (player == null)
        {
            GameObject temp = FindPlayerObject();
            if (temp != null)
            {
                player = temp.transform;
                playerMovement = temp.GetComponent<movement>();
            }
        }

        if (player != null)
        {
            lastPlayerDistance = Vector2.Distance(rb.position, player.position);
        }
    }

    public void SetPlayerInVision(bool visible)
    {
        playerInVision = visible;
        if (!visible)
        {
            isFollowingPlayer = false;
        }
    }

    GameObject FindPlayerObject()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        return temp;
    }

}
