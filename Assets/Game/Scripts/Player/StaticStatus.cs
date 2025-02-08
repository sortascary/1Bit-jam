using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaticStatus : MonoBehaviour
{
    private static StaticStatus _instance;
    public static StaticStatus Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<StaticStatus>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("StaticStatus");
                    _instance = obj.AddComponent<StaticStatus>();
                }
            }
            return _instance;
        }
    }

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;

    [Header("Energy Settings")]
    [SerializeField] private float _maxEnergy = 100f;
    [SerializeField] private float _currentEnergy = 100f;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI _ScoreTime;

    [Header("Movement Settings")]
    [SerializeField] private float _accelerationForce = 10f;
    [SerializeField] private float _maxSpeed = 15f;
    [SerializeField] private float _maxSpeedDash = 45f;
    [SerializeField] private float _decelerationRate = 5f;
    [SerializeField] private float _turningSpeed = 5f;
    [SerializeField] private float _dashForce = 5f;
    [SerializeField] private float _dashCooldown = 0.4f;
    [SerializeField] private float _dashDuration = 0.5f;

    [Header("Bleed Settings")]
    [SerializeField] private float _BleedAmount = 0.03f;
    [SerializeField] private float _HealAmount = 2f;

    [Header("Bullet Settings")]
    [SerializeField] private float _bulletSpeedMultiplier = 1f;
    [SerializeField] private float _bulletCooldownMultiplier = 1f;

    [Header("Endscreen settings")]
    [SerializeField] private Image _endScreen;
    [SerializeField] private TextMeshProUGUI _ScoreTi;

    private bool _isInteracting = false;

    public static float MaxHealth
    {
        get => Instance._maxHealth;
        set => Instance._maxHealth = value;
    }

    public static float CurrentHealth
    {
        get => Instance._currentHealth;
        set => Instance._currentHealth = value;
    }

    public static float MaxEnergy
    {
        get => Instance._maxEnergy;
        set => Instance._maxEnergy = value;
    }

    public static float CurrentEnergy
    {
        get => Instance._currentEnergy;
        set => Instance._currentEnergy = value;
    }

    public static TextMeshProUGUI ScoreTime
    {
        get => Instance._ScoreTime;
        set => Instance._ScoreTime = value;
    }

    public static float AccelerationForce
    {
        get => Instance._accelerationForce;
        set => Instance._accelerationForce = value;
    }

    public static float MaxSpeed
    {
        get => Instance._maxSpeed;
        set => Instance._maxSpeed = value;
    }

    public static float MaxSpeedDash
    {
        get => Instance._maxSpeedDash;
        set => Instance._maxSpeedDash = value;
    }

    public static float DecelerationRate
    {
        get => Instance._decelerationRate;
        set => Instance._decelerationRate = value;
    }

    public static float TurningSpeed
    {
        get => Instance._turningSpeed;
        set => Instance._turningSpeed = value;
    }

    public static float DashForce
    {
        get => Instance._dashForce;
        set => Instance._dashForce = value;
    }

    public static float DashCooldown
    {
        get => Instance._dashCooldown;
        set => Instance._dashCooldown = value;
    }

    public static float DashDuration
    {
        get => Instance._dashDuration;
        set => Instance._dashDuration = value;
    }

    public static float BleedAmount
    {
        get => Instance._BleedAmount;
        set => Instance._BleedAmount = value;
    }

    public static float HealAmount
    {
        get => Instance._HealAmount;
        set => Instance._HealAmount = value;
    }

    public static float BulletSpeedMultiplier
    {
        get => Instance._bulletSpeedMultiplier;
        set => Instance._bulletSpeedMultiplier = value;
    }

    public static float BulletCooldownMultiplier
    {
        get => Instance._bulletCooldownMultiplier;
        set => Instance._bulletCooldownMultiplier = value;
    }

    public static Image EndScreen
    {
        get => Instance._endScreen;
        set => Instance._endScreen = value;
    }

    public static TextMeshProUGUI ScoreTi
    {
        get => Instance._ScoreTi;
        set => Instance._ScoreTi = value;
    }

    public static bool IsInteracting
    {
        get => Instance._isInteracting;
        set => Instance._isInteracting = value;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
