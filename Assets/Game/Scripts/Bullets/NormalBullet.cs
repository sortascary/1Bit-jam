using UnityEngine;

public class NormalBullet : BulletBehavior
{
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float bulletDamage = 1;
    [SerializeField] private float bulletCost = 1;
    [SerializeField] private float cooldown = 0.1f;
    [SerializeField] private float timeToDestroy = 3;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;

    private AudioSource src;
    private Rigidbody2D rb;
    private bool firstHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!firstHit)
        {
            src = GetComponent<AudioSource>();

            src.clip = hitSound;
            src.pitch = Random.Range(1.5f, 2.2f);

            src.Play();

            if (hitEffect != null) Destroy(Instantiate(hitEffect, transform.position, transform.rotation), 0.5f);

            IsDamage isDamage = collision.gameObject.GetComponent<IsDamage>();
            if (isDamage != null)
            {
                isDamage.Damage(bulletDamage);
            }

            Destroy(transform.GetChild(0).gameObject);
        }

        firstHit = true;

        if (!src.isPlaying) Destroy(gameObject);
    }

    public override void Attack(float bulletSpeedMultiplier, GameObject player)
    {
        Destroy(gameObject, timeToDestroy);

        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = transform.up * bulletSpeed * bulletSpeedMultiplier;
    }

    public override float GetCooldown(float bulletCooldownMultiplier)
    {
        return cooldown * bulletCooldownMultiplier;
    }

    public override float EnergyCost()
    {
        return bulletCost;
    }
}
