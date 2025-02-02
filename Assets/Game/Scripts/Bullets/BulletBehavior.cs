using UnityEngine;

public abstract class BulletBehavior : MonoBehaviour
{
    public virtual void Attack(float bulletSpeedMultiplier)
    {
        Debug.Log("No Attack Detected");
    }

    public virtual float GetCooldown(float bulletCooldownMultiplier)
    {
        return 0.5f;
    }

    public virtual float EnergyCost()
    {
        return 1f;
    }
}
