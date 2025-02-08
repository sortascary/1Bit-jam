using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IsDamage
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHealth = 3f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float safeDistance = 7f;
    [SerializeField] private float teleportThreshold = 25f; // Max distance before teleporting
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Teleportation Settings")]
    [SerializeField] private float teleportRadius = 12f; // Distance for teleportation
    [SerializeField] private int maxTeleportAttempts = 10;

    [Header("Movement Settings")]
    [SerializeField] private float subtleMovementIntensity = 1f;
    [SerializeField] private float subtleMovementFrequency = 2f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float turningSpeed = 5f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float upgradeBulletSpeed = 5f;
    [SerializeField] private float shootingCooldown = 2f;
    [SerializeField] private float shootingAngleThreshold = 15f;

    [Header("Loot Settings")]
    [SerializeField] private GameObject[] loots;
    [SerializeField] private float[] lootChances = { 30f, 60f, 10f };

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private float currentHealth;
    private Vector2 subtleMovementOffset;
    private float subtleMovementTimer;
    private float currentCooldown = 0f;
    private Camera mainCamera;
    public bool CanMove { get; set; } = true;


    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {

        DetectPlayer();
        if (CanMove)
        {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Flip sprite based on movement direction
            if (rb.linearVelocity.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (rb.linearVelocity.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            if (distanceToPlayer > teleportThreshold)
            {
                TryTeleport();
            }
            else
            {
                MaintainSafeDistance();
                ApplySubtleMovement();

                if (currentCooldown <= 0f && CanShootPlayer())
                {
                    ShootAtPlayer();
                    currentCooldown = shootingCooldown;
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        if (currentCooldown > 0f)
        {
            currentCooldown -= Time.deltaTime;
        }


        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        player = playerCollider != null ? playerCollider.transform : null;
    }

    private void MaintainSafeDistance()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < safeDistance)
        {
            Vector2 directionAway = ((Vector2)transform.position - (Vector2)player.position).normalized;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, directionAway * speed, Time.deltaTime);
        }
        else if (distanceToPlayer > safeDistance + 2f)
        {
            Vector2 directionToward = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, directionToward * speed, Time.deltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime);
        }
    }

    private void ApplySubtleMovement()
    {
        subtleMovementTimer += Time.deltaTime;
        if (subtleMovementTimer >= subtleMovementFrequency)
        {
            subtleMovementOffset = new Vector2(
                Random.Range(-subtleMovementIntensity, subtleMovementIntensity),
                Random.Range(-subtleMovementIntensity, subtleMovementIntensity)
            );
            subtleMovementTimer = 0;
        }

        rb.AddForce(subtleMovementOffset, ForceMode2D.Force);
    }

    private bool CanShootPlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if player is within shooting radius
        if (distanceToPlayer > detectionRadius) return false;

        // Check if there are obstacles blocking the shot
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        return hit.collider == null || hit.collider.transform == player;
    }


    private void ShootAtPlayer()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null || player == null)
        {
            Debug.LogWarning("Bullet prefab, spawn point, or player is not set!");
            return;
        }

        animator.SetTrigger("throw");
        // Calculate the direction towards the player
        Vector2 directionToPlayer = (player.position - bulletSpawnPoint.position).normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        // Rotate the bullet to face the player
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Apply force/movement to the bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = directionToPlayer * upgradeBulletSpeed;
        }

        Destroy(bullet, 3f);
    }



    private void TryTeleport()
    {
        Debug.Log("Attempting to teleport...");

        for (int i = 0; i < maxTeleportAttempts; i++)
        {
            // Generate a random offset around the player
            Vector2 randomOffset = Random.insideUnitCircle.normalized * teleportRadius;
            Vector2 teleportPosition = (Vector2)player.position + randomOffset;

            // Check if the position is off-screen and not inside an obstacle
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

        bool isVisible = viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;

        Debug.Log("Checking visibility at " + position + " -> Visible: " + isVisible);

        return isVisible;
    }


    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            SpawnLoot();
            Destroy(gameObject);
        }
    }

    private void SpawnLoot()
    {
        if (loots == null || loots.Length == 0) return;

        float totalChance = 0;
        foreach (float chance in lootChances) totalChance += chance;

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0;

        for (int i = 0; i < lootChances.Length; i++)
        {
            cumulativeChance += lootChances[i];
            if (randomValue <= cumulativeChance)
            {
                if (i < loots.Length && loots[i] != null)
                {
                    Instantiate(loots[i], transform.position, Quaternion.identity);
                }
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}
