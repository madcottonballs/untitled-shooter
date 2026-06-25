using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class attack : MonoBehaviour
{
    [SerializeField] float lungeDistance = 2.5f;
    [SerializeField] float lungeDuration = 0.15f;
    [SerializeField] float lungeRecoveryDuration = 0.1f;
    [SerializeField] AnimationCurve lungeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Rigidbody2D rb;
    movement playerMovement;

    public bool isLunging;
    bool isRecovering;
    float lungeElapsed;
    Vector2 lungeStartPosition;
    Vector2 lungeEndPosition;
    Vector2 facingDirection = Vector2.right;
    public int lungeDamage = 100;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<movement>();
    }

    void Update()
    {
        // Press Q to trigger the lunge.
        if (Input.GetKeyDown(KeyCode.Q) && !isLunging && !isRecovering)
        {
            StartLunge();
        }

        // Remember the last movement direction so the lunge has a sensible direction
        // even if the player is standing still when the key is pressed.
        if (!isLunging && !isRecovering && playerMovement != null && playerMovement.velocity.sqrMagnitude > 0.001f)
        {
            facingDirection = playerMovement.velocity.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!isLunging) // means all following code in this function only runs if lunging
        {
            return;
        }

        lungeElapsed += Time.fixedDeltaTime;

        float progress = Mathf.Clamp01(lungeElapsed / lungeDuration);
        float easedProgress = lungeCurve.Evaluate(progress);

        // Move the Rigidbody through the physics step so collision resolution stays stable.
        rb.MovePosition(Vector2.Lerp(lungeStartPosition, lungeEndPosition, easedProgress));

        if (progress >= 1f)
        {
            isLunging = false;
            StartCoroutine(FinishLunge());
        }
    }

    void StartLunge()
    {
        Vector2 direction = facingDirection.sqrMagnitude > 0.001f ? facingDirection.normalized : Vector2.right;

        lungeStartPosition = rb.position;
        lungeEndPosition = lungeStartPosition + direction * lungeDistance;
        lungeElapsed = 0f;
        isLunging = true;

        // Stop the normal movement script so it does not fight the lunge motion.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Clear existing motion so the lunge starts cleanly.
        rb.velocity = Vector2.zero;
    }

    IEnumerator FinishLunge()
    {
        isRecovering = true;

        // Optional small pause so the lunge feels intentional instead of instant.
        yield return new WaitForSecondsRealtime(lungeRecoveryDuration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isRecovering = false;
    }
}
