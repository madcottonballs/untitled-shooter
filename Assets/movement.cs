using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class movement : MonoBehaviour
{
    Rigidbody2D rb;
    CanvasGroupFader fadeController;
    float horizontalDirection, verticalDirection;
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 20f;
    public int level = 1;
    bool isTransitioning;
    // Current movement speed and direction for this frame.
    Vector2 velocity;
    Vector2 inputDirection;
    private void OnTriggerEnter2D(Collider2D other) // handles collision
    {
        if (!isTransitioning && other.CompareTag("wintilemap")) // detects win condition
        {
            StartCoroutine(LoadNextLevelAfterFade());
        }
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        fadeController = FindObjectOfType<CanvasGroupFader>();
    }

    IEnumerator LoadNextLevelAfterFade()
    {
        isTransitioning = true;
        enabled = false;

        if (fadeController != null)
        {
            fadeController.FadeOut();
            yield return new WaitForSecondsRealtime(fadeController.FadeOutDuration);
        }

        level++;
        SceneManager.LoadScene("Level " + level);
    }

    void Update()
    {
        // Read raw input so movement snaps immediately to -1, 0, or 1.
        horizontalDirection = Input.GetAxisRaw("Horizontal");
        verticalDirection = Input.GetAxisRaw("Vertical");
        // Cache the desired direction so physics can apply it in FixedUpdate.
        inputDirection = new Vector2(horizontalDirection, verticalDirection).normalized;
    }

    void FixedUpdate()
    {
        // Build the desired movement direction on the 2D plane.
        Vector2 direction = inputDirection;

        // Convert direction into the speed we want to reach this frame.
        Vector2 targetVelocity = direction * maxSpeed;

        // if acceleration, using acceleration rate, if decelerating, use deceleration rate
        float currentRate = direction == Vector2.zero ? deceleration : acceleration;

        // Ease the current velocity toward the target velocity so acceleration
        // and deceleration feel smooth instead of instant.
        velocity = Vector2.MoveTowards(velocity, targetVelocity, currentRate * Time.fixedDeltaTime);

        // Move the Rigidbody during the physics step so collisions stay stable.
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}
