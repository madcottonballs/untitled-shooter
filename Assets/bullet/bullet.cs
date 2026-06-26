using UnityEngine;

public class bullet : MonoBehaviour
{
    public int damage = 250; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("wall"))
        {
            Destroy(gameObject);
        }        

        else if (other.CompareTag("enemy"))
        {
            t1enemyMovement enemy = other.GetComponent<t1enemyMovement>(); // to get access to the health field

            enemy.health -= damage; 

            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            attack player = other.GetComponent<attack>(); // to get access to the health field

            player.health -= damage; 

            Destroy(gameObject);
        }        

    }
}
