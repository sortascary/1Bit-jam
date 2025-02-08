using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private float knockbackMultiplier = 100;
    [SerializeField] private float bulletDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsDamage isDamage = collision.gameObject.GetComponent<IsDamage>();
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * knockbackMultiplier);
        if (isDamage != null)
        {
            isDamage.Damage(bulletDamage);
        }
    }
}
