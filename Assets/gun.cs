using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : MonoBehaviour
{
    [SerializeField] float offset = .25f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 12f;
    [SerializeField] float bulletLifetime = 2f;
    [SerializeField] float bulletSpawnOffset = 0.2f;
    [SerializeField] int bulletDamage = 250;

    void Update()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        // Convert the mouse position into world space on the same Z plane as the player.
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = parentTransform.position.z;

        // Use the direction from the player to the mouse, not from the gun to the mouse.
        Vector2 direction = ((Vector2)mousePos - (Vector2)parentTransform.position).normalized;

        // Keep the gun on a fixed radius around the parent so it orbits the player cleanly.
        transform.position = parentTransform.position + (Vector3)(direction * offset);

        // Point the gun at the mouse.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ensure the gun is flipped correctly
        if (mousePos.x < parentTransform.position.x)
        {
            sr.flipX = true;
            sr.flipY = true;
        }
        else
        {
            sr.flipX = true;
            sr.flipY = false;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void shoot()
    {
        if (bulletPrefab == null)
        {
            return;
        }

        // Spawn the bullet slightly in front of the gun so it does not overlap the player.
        Vector3 spawnPosition = transform.position + transform.right * bulletSpawnOffset;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);

        // Make sure the bullet can actually move and collide.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bullet bulletScript = bullet.GetComponent<bullet>();

        bulletRb.velocity = (Vector2)transform.right * bulletSpeed;
        bulletScript.damage = bulletDamage; // sets the bullets damage to the bulletDamage


        Destroy(bullet, bulletLifetime);
    }
}
