using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBullet : BulletBehavior
{
    LineRenderer lineRenderer;

    [SerializeField] private float startLaserSize = 0.1f;
    [SerializeField] private float endLaserSize = 1f;
    [SerializeField] private float length = 30f;
    [SerializeField] private float timeToShoot = 0.4f;
    [SerializeField] private float bulletDamage = 1;
    [SerializeField] private float bulletCost = 1;
    [SerializeField] private float cooldown = 0.1f;
    [SerializeField] private float timeToDestroy = 3;
    [SerializeField] private float secondsToDamage;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;

    private AudioSource src;
    private bool firstTime = false;
    private bool hasShot = false;
    private Vector2 lastShotPosition;
    private Vector2 lastEndPosition;
    private GameObject playerObject;
    Vector3 endPosition;
    private float laserSize;
    private float shotStartTime;
    private float hasShotStartTime;
    private float lineLength;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Attack(float bulletSpeedMultiplier, GameObject player)
    {
        Destroy(gameObject, timeToDestroy);

        StartCoroutine(UntilShot());

        playerObject = player;
        shotStartTime = Time.time;

        Vector2 direction = transform.up;
        lineLength = bulletSpeedMultiplier * length;
        endPosition = transform.position + (Vector3)direction * lineLength;
    }

    private void Update()
    {
        if (lineRenderer == null || playerObject == null) return;

        if (hasShot)
        {
            float t = Mathf.Clamp01((Time.time - hasShotStartTime) / (timeToDestroy - timeToShoot));
            float alpha = Mathf.Lerp(1f, 0f, t);

            Color fadedColor = new Color(1f, 1f, 1f, alpha);
            lineRenderer.startColor = fadedColor;
            lineRenderer.endColor = fadedColor;

            lineRenderer.SetPosition(0, lastShotPosition);
            lineRenderer.SetPosition(1, lastEndPosition);

            if(!firstTime) StartCoroutine(Damage());
        }
        else
        {
            float t = Mathf.Clamp01((Time.time - shotStartTime) / timeToDestroy);
            laserSize = Mathf.Lerp(startLaserSize, endLaserSize, t);
            lineRenderer.startWidth = laserSize;
            lineRenderer.endWidth = laserSize;
            lineRenderer.SetPosition(0, playerObject.transform.position);
            lineRenderer.SetPosition(1, endPosition + playerObject.transform.position);
            lastShotPosition = playerObject.transform.position;
            lastEndPosition = endPosition + playerObject.transform.position;
            hasShotStartTime = Time.time;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(lastShotPosition ,lastEndPosition);
    }
    IEnumerator UntilShot()
    {
        yield return new WaitForSeconds(timeToShoot);
        hasShot = true;
    }

    IEnumerator Damage()
    {
        firstTime = true;

        yield return new WaitForSeconds(secondsToDamage);

        RaycastHit2D hit = Physics2D.Raycast(lastShotPosition, transform.up, lineLength);

        if (hit)
        {
            IsDamage isDamage = hit.collider.GetComponent<IsDamage>();
            if (isDamage != null)
            {
                isDamage.Damage(bulletDamage);
            }
        }
        StartCoroutine(Damage());
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
