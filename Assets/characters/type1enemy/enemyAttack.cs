using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAttack : MonoBehaviour
{
    [SerializeField] bool hasGun = false;
    [SerializeField] t1enemyMovement enemyMovement;
    [SerializeField] float reloadTime = 0.5f;
    [SerializeField] float stunTime = 0.6f; // the time in seconds that the enemy is startled by finding the player before shooting
    bool shooting;
    // Start is called before the first frame update
    void Update()
    {
        if (hasGun && enemyMovement.awareOfPlayer && !shooting) // if enemy has gun, aware of player, and not shooting already
        {
            gun gunMaster = GetComponentInChildren<gun>();

            StartCoroutine(shootAtPlayer(gunMaster));
        }
    }

    IEnumerator shootAtPlayer(gun gunMaster)
    {
        shooting = true;
        yield return new WaitForSeconds(stunTime);
        while (hasGun && enemyMovement.awareOfPlayer && enemyMovement.playerInVision)
        {
            gunMaster.shoot();
            yield return new WaitForSeconds(reloadTime);
        }
        shooting = false;
    }
}
