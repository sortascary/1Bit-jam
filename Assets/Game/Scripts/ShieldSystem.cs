using UnityEngine;

public class ShieldSystem : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private GameObject shieldVisual; // Visual representation of the shield
    [SerializeField] private float shieldDuration = 5f; // How long the shield lasts
    [SerializeField] private float shieldCooldown = 10f; // Cooldown before the shield can be used again


    // Handle collisions with the shield
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            Destroy(collision.gameObject);
        }

        // Check if the collided object is a melee enemy
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.CanMove = false; 
            }
        }
        }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the collided object is a melee enemy
        if (collision.CompareTag("Enemy"))
        {
            EnemyBehavior enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                enemy.CanMove = true;
            }
        }
    }
}