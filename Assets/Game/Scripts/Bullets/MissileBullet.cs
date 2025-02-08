using UnityEngine;

public class MissileBullet : BulletBehavior
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float bulletDamage = 1;
    [SerializeField] private float knockbackMultiplier;
    [SerializeField] private float bulletCost = 1;
    [SerializeField] private float cooldown = 0.1f;
    [SerializeField] private float timeToDestroy = 3;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;

    [Header("Homing Settings")]
    [SerializeField] private float turnRate = 90f; // Degrees per second
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float detectionAngle = 45f; // Degrees
    [SerializeField] private LayerMask enemyLayer;

    private AudioSource src;
    private Rigidbody2D rb;
    private bool firstHit = false;
    private Transform target;
    private float speedMultiplier;

    private void FixedUpdate()
    {
        if (firstHit) return;

        // Update target if current target is invalid
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindNearestEnemy();
        }

        if (target != null)
        {
            Vector2 currentDir = rb.linearVelocity.normalized;
            Vector2 toTargetDir = (target.position - transform.position).normalized;

            // Calculate steering
            float angle = Vector2.SignedAngle(currentDir, toTargetDir);
            float maxTurnAngle = turnRate * Time.fixedDeltaTime;
            float turnAngle = Mathf.Clamp(angle, -maxTurnAngle, maxTurnAngle);

            // Apply rotation to direction
            Quaternion rotation = Quaternion.AngleAxis(turnAngle, Vector3.forward);
            Vector2 newDir = rotation * currentDir;

            // Update velocity and rotation
            rb.linearVelocity = newDir * bulletSpeed * speedMultiplier;
            UpdateRotation(newDir);
        }
    }

    private void UpdateRotation(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void FindNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            Vector2 directionToEnemy = (hit.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, directionToEnemy);

            if (angle <= detectionAngle)
            {
                float distance = Vector2.SqrMagnitude(hit.transform.position - transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hit.transform;
                }
            }
        }

        target = closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!firstHit)
        {
            src = GetComponent<AudioSource>();
            src.clip = hitSound;
            src.pitch = Random.Range(1.5f, 2.2f);
            src.Play();

            if (hitEffect != null)
                Destroy(Instantiate(hitEffect, transform.position, transform.rotation), 0.5f);

            IsDamage isDamage = collision.gameObject.GetComponent<IsDamage>();
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * knockbackMultiplier);
            if (isDamage != null)
            {
                isDamage.Damage(bulletDamage);
            }

            Destroy(gameObject);
        }

        firstHit = true;
        if (!src.isPlaying) Destroy(gameObject);
    }

    public override void Attack(float bulletSpeedMultiplier, GameObject player)
    {
        Destroy(gameObject, timeToDestroy);
        rb = GetComponent<Rigidbody2D>();
        this.speedMultiplier = bulletSpeedMultiplier;
        rb.linearVelocity = transform.up * bulletSpeed * speedMultiplier;
        FindNearestEnemy(); // Initial target acquisition
    }

    public override float GetCooldown(float bulletCooldownMultiplier)
    {
        return cooldown * bulletCooldownMultiplier;
    }

    public override float EnergyCost()
    {
        return bulletCost;
    }

    // Visualize detection range and angle in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftDir = Quaternion.Euler(0, 0, detectionAngle) * transform.up;
        Vector3 rightDir = Quaternion.Euler(0, 0, -detectionAngle) * transform.up;

        Gizmos.DrawRay(transform.position, leftDir * detectionRadius);
        Gizmos.DrawRay(transform.position, rightDir * detectionRadius);
    }
}