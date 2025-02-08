using UnityEngine;
using UnityEngine.UI;

public class StatusAdapter : MonoBehaviour
{
    public static StatusAdapter Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Health References")]
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;


    [Header("Animation")]
    [SerializeField] private Color restoreHealthColor;
    [SerializeField] private Color damageHealthColor;
    [SerializeField] private Color restoreEnergyColor;
    [SerializeField] private Color damageEnergyColor;
    [SerializeField] public float chipSpeed = 2f;
    private float healthLerpTimer;
    private float energyLerpTimer;
    private float healthWaitTime;
    private float energyWaitTime;

    private void Start()
    {
        StaticStatus.CurrentHealth = StaticStatus.MaxHealth;
        StaticStatus.CurrentEnergy = StaticStatus.MaxEnergy;
    }

    private void Update()
    {
        StaticStatus.CurrentHealth = Mathf.Clamp(StaticStatus.CurrentHealth, 0, StaticStatus.MaxHealth);
        StaticStatus.CurrentEnergy = Mathf.Clamp(StaticStatus.CurrentEnergy, 0, StaticStatus.MaxEnergy);
        UpdateHealthUI();
    }

    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = StaticStatus.CurrentHealth / StaticStatus.MaxHealth;
        healthWaitTime += Time.deltaTime;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = damageHealthColor;
            healthLerpTimer += Time.deltaTime;
            float percentComplete = healthLerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = restoreHealthColor;
            backHealthBar.fillAmount = hFraction;
            healthLerpTimer += Time.deltaTime;
            float percentComplete = healthLerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float amount)
    {
        StaticStatus.CurrentHealth -= amount;
        healthLerpTimer = 0;
        healthWaitTime = 0;
    }

    public void RestoreHealth(float amount)
    {
        StaticStatus.CurrentHealth += amount;
        healthLerpTimer = 0;
        healthWaitTime = 0;
    }
}