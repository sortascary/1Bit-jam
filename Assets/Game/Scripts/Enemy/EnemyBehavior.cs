using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IsDamage
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxHealth = 3f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float safeDistance = 7f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

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
    private Rigidbody2D rb;
    private float currentHealth;
    private Vector2 subtleMovementOffset;
    private float subtleMovementTimer;
    private float currentCooldown = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Debug.Log(currentHealth);

        DetectPlayer();

        if (player != null)
        {
            MaintainSafeDistance();
            ApplySubtleMovement();

            if (currentCooldown <= 0f && CanShootPlayer())
            {
                ShootAtPlayer();
                currentCooldown = shootingCooldown;
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

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if (playerCollider != null)
        {
            player = playerCollider.transform;
        }
        else
        {
            player = null;
        }
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

        Vector2 lookDirection = (Vector2)player.position - (Vector2)transform.position;
        if (lookDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle), turningSpeed * Time.deltaTime);
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

        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float angleToPlayer = Vector2.Angle(transform.up, directionToPlayer);
        if (angleToPlayer > shootingAngleThreshold) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, obstacleLayer);
        if (hit.collider == null || hit.collider.transform == player)
        {
            return true;
        }

        return false;
    }

    private void ShootAtPlayer()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogWarning("Bullet prefab or spawn point is not set!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Attack(upgradeBulletSpeed, gameObject);
        }

        Destroy(bullet, 3f);
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
