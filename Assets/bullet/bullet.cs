using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] LayerMask destroyOnLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
