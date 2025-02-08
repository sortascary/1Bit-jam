using UnityEngine;

public class EnemyBehaviorMelee : MonoBehaviour, IsDamage
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float maxHealth = 5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRadius = 2f;
    [SerializeField] private float teleportThreshold = 20f; // Distance before teleporting
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Teleportation Settings")]
    [SerializeField] private float teleportRadius = 8f; // Distance for teleportation
    [SerializeField] private int maxTeleportAttempts = 10;
    
    [Header("Attack Settings")]
    [SerializeField] private GameObject attackObject; // Attack hitbox
    [SerializeField] private float attackCooldown = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private float currentHealth;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private Camera mainCamera;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        if (attackObject != null) attackObject.SetActive(false); // Deactivate attack initially
    }

    void Update()
    {
        DetectPlayer();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > teleportThreshold)
            {
                TryTeleport();
            }
            else if (distanceToPlayer <= attackRadius)
            {
                if (!isAttacking)
                {
                    Attack();
                }
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                if (attackObject != null) attackObject.SetActive(false);
            }
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        player = playerCollider != null ? playerCollider.transform : null;
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);  // Facing right
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Facing left
    }

    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        if (attackObject != null)
        {
            attackObject.SetActive(true);
        }

        Debug.Log("Enemy Attacking!");
    }

    private void TryTeleport()
    {
        Debug.Log("Attempting to teleport...");

        for (int i = 0; i < maxTeleportAttempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * teleportRadius;
            Vector2 teleportPosition = (Vector2)player.position + randomOffset;

            if (!IsPositionVisible(teleportPosition) && !Physics2D.OverlapCircle(teleportPosition, 1f, obstacleLayer))
            {
                transform.position = teleportPosition;
                Debug.Log("Teleported to: " + teleportPosition);
                return;
            }
        }

        Debug.LogWarning("No valid teleport location found after " + maxTeleportAttempts + " attempts.");
    }

    private bool IsPositionVisible(Vector2 position)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(position);
        return viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
