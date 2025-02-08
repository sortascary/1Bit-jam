using System.Collections;
using UnityEngine;

public class AnimalBehavior : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float fleeSpeed = 5f;
    public float wanderRadius = 5f;
    public float disappearTime = 30f;
    public float delayTime = 2f;
    public bool isDead = false;
    public float Healammount;

    public bool isPlayerNearby = false;
    private float timer = 0f;
    private Vector2 wanderTarget;

    void Start()
    {
        SetRandomWanderTarget();
    }

    void Update()
    {
        if (isDead) return;
        if (isPlayerNearby)
        {
            // Hewan menjauh dari player
            FleeFromPlayer();
            timer = 0f; // Reset timer saat player ada di dekatnya
        }
        else
        {
            // Hewan berkeliaran jika tidak ada player
            WanderAround();

            // Hitung mundur untuk menghilang
            timer += Time.deltaTime;
            if (timer >= disappearTime)
            {
                gameObject.SetActive(false); // Menghilangkan hewan setelah 30 detik
            }
        }
    }

    public void IsDie()
    {
        isDead = true;
        StatusAdapter.Instance.RestoreHealth(Healammount);
        //gambar animasi mati
    }

    void FleeFromPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 fleeDirection = (transform.position - player.transform.position).normalized;
            transform.position += (Vector3)fleeDirection * fleeSpeed * Time.deltaTime;
        }
    }

    void WanderAround()
    {
        if ((Vector2)transform.position == wanderTarget)
        {
            SetRandomWanderTarget();
        }

        transform.position = Vector2.MoveTowards(transform.position, wanderTarget, moveSpeed * Time.deltaTime);
    }

    void SetRandomWanderTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * wanderRadius;
        wanderTarget = (Vector2)transform.position + randomDirection;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DelayResetPlayerNearby());
        }
    }

    IEnumerator DelayResetPlayerNearby()
    {
        yield return new WaitForSeconds(delayTime);
        isPlayerNearby = false;
    }
    void OnDrawGizmosSelected()
    {
        // Set warna untuk gizmo
        Gizmos.color = Color.green;

        // Gambar lingkaran dengan radius wanderRadius
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

}
