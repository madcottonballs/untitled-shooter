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
    Coroutine lungeRoutine;
    bool isLunging;
    Vector2 facingDirection = Vector2.right;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<movement>();
    }

    void Update()
    {
        // Press Space to trigger the lunge. Replace this with your own input if needed.
        if (Input.GetKeyDown(KeyCode.Q) && !isLunging)
        {
            StartLunge();
        }

        // Keep the last facing direction so the lunge still works when standing still.
        if (!isLunging && playerMovement.velocity.sqrMagnitude > 0.001f)
        {
            facingDirection = playerMovement.velocity.normalized;
        }
    }

    public void StartLunge()
    {
        if (lungeRoutine != null)
        {
            StopCoroutine(lungeRoutine);
        }

        lungeRoutine = StartCoroutine(Lunge());
    }

    IEnumerator Lunge()
    {
        isLunging = true;

        // Use the last known movement direction, or default to right if the player was idle.
        Vector2 direction = facingDirection.sqrMagnitude > 0.001f ? facingDirection.normalized : Vector2.right;
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + direction * lungeDistance;

        // Lock out normal movement for the duration of the lunge.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // First half: move forward quickly with easing.
        yield return MoveBetweenPoints(startPosition, targetPosition, lungeDuration);

        // do damage to enemy

        // Second half: optional recovery pause so the motion feels intentional instead of snappy.
        yield return new WaitForSecondsRealtime(lungeRecoveryDuration);

        // Re-enable regular movement after the lunge finishes.
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isLunging = false;
        lungeRoutine = null;
    }

    IEnumerator MoveBetweenPoints(Vector2 startPosition, Vector2 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Ease the motion so the lunge starts fast and slows near the end.
            float easedProgress = lungeCurve.Evaluate(progress);
            rb.MovePosition(Vector2.Lerp(startPosition, endPosition, easedProgress));

            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(endPosition);
    }
}
