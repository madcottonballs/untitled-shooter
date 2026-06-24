using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    float horizontalDirection, verticalDirection;
    public float maxSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 20f;
    // Current movement speed and direction for this frame.
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        // Reset the player to the world origin when the scene starts.
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Read raw input so movement snaps immediately to -1, 0, or 1.
        horizontalDirection = Input.GetAxisRaw("Horizontal");
        verticalDirection = Input.GetAxisRaw("Vertical");
        // Build the desired movement direction on the X/Y plane.
        Vector3 direction = new Vector3(horizontalDirection, verticalDirection, 0f).normalized;

        // Convert direction into the speed we want to reach this frame.
        Vector3 targetVelocity = direction * maxSpeed;

        // if acceleration, using acceleration rate, if decelerating, use deceleration rate
        float currentRate = direction == Vector3.zero ? deceleration : acceleration;

        // Ease the current velocity toward the target velocity so acceleration
        // and deceleration feel smooth instead of instant.
        velocity = Vector3.MoveTowards(velocity, targetVelocity, currentRate * Time.deltaTime);

        // Move by velocity multiplied by deltaTime so motion stays frame-rate independent.
        transform.position += velocity * Time.deltaTime;
    }
}
