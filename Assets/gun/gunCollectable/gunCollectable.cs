using UnityEngine;

public class gunCollectable : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] PickupPromptUI pickupPrompt;
    [SerializeField] string pickupMessage = "Press P to pick up gun.";
    [SerializeField] GameObject gunPrefab;
    bool playerInRange;

    void Awake()
    {
        if (pickupPrompt == null)
        {
            pickupPrompt = FindObjectOfType<PickupPromptUI>();
        }
    }

    public void InitializeFromGun(Transform sourceTransform, SpriteRenderer sourceSprite) // spawns in
    {
        if (sourceTransform != null)
        {
            transform.SetPositionAndRotation(sourceTransform.position, sourceTransform.rotation);
        }

        if (sr != null && sourceSprite != null)
        {
            sr.flipX = sourceSprite.flipX;
            sr.flipY = sourceSprite.flipY;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {

            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];

            attack playerAttackScript = player.GetComponent<attack>();


            GameObject newGun = Instantiate(gunPrefab, player.transform);
            gun newGunScript = newGun.GetComponent<gun>();

            playerAttackScript.gun = newGunScript;

            playerAttackScript.hasGun = true;

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;

        if (pickupPrompt != null)
        {
            pickupPrompt.Show(pickupMessage);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;

        if (pickupPrompt != null)
        {
            pickupPrompt.Hide();
        }
    }
}
