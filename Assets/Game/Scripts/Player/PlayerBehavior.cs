using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerBehavior : MonoBehaviour, IsDamage
{
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputAsset;
    private InputActionMap gameplayInputMap;
    private InputAction moveAction;
    private InputAction interactAction;
    [HideInInspector] public InputAction mouseAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public InputAction attackAction;
    [HideInInspector] public InputAction dashAction;

    [Header("Bullet Settings")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Internal State")]
    private float currentMaxSpeed;
    private Vector2 moveDirection;
    [HideInInspector] public Vector2 targetPlayerRotation;
    private Rigidbody2D rb;
    private Animator animator;
    [HideInInspector] public bool controllerConnected = false;
    private float _scoreTimer = 0f;
    private float scoreTi;
    private float currentShootCooldown = 0f;
    [HideInInspector] public float currentDashCooldown = 0f;

    [Header("Shield Settings")]
    public float currentShieldCooldown = 0f;
    public float shieldCooldownDuration = 5f;
    public GameObject shield;

    [Header("Score")]
    public float score = 0;
    public TextMeshProUGUI scoreText;
    public float totalScore = 0;
    public TextMeshProUGUI totalScoreText;
    public Vector2 MoveDirection { get => moveDirection; set => moveDirection = value; }

    private void Awake()
    {
        if (StaticStatus.Instance == null)
        {
            Debug.LogError("StaticStatus is not set. Ensure it is in the scene.");
        }
        if (StaticStatus.EndScreen != null) StaticStatus.EndScreen.gameObject.SetActive(false);
        SetupReferences();
        SetUpBindings();
        StartCoroutine(CheckForControllers());
        shield.SetActive(false);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        interactAction?.Enable();
        mouseAction?.Enable();
        lookAction?.Enable();
        attackAction?.Enable();
        dashAction?.Enable();

        if (attackAction != null)
        {
            attackAction.performed += OnAttack;
        }

        if (dashAction != null)
        {
            dashAction.performed += OnDash;
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.Disable();
            attackAction.performed -= OnAttack;
        }

        if (dashAction != null)
        {
            dashAction.Disable();
            dashAction.performed -= OnDash;
        }

        moveAction?.Disable();
        interactAction?.Disable();
        mouseAction?.Disable();
        lookAction?.Disable();
    }

    private void Update()
    {
        if (StaticStatus.CurrentHealth > 0)
        {
            _scoreTimer += Time.deltaTime; // Tambahkan waktu hanya jika masih hidup
        }
        else
        {
            if (StaticStatus.ScoreTi != null)
            {
                StaticStatus.ScoreTi.text = Mathf.FloorToInt(_scoreTimer).ToString();
            }
        }

        if (StaticStatus.ScoreTime != null)
        {
            // Tampilkan waktu yang berjalan di gameplay
            int _GameMinutes = Mathf.FloorToInt(_scoreTimer / 60);
            int _GameSeconds = Mathf.FloorToInt(_scoreTimer % 60);
            StaticStatus.ScoreTime.text = $"{_GameMinutes:00} : {_GameSeconds:00}";
        }

        if (currentShieldCooldown > 0f)
        {
            currentShieldCooldown -= Time.deltaTime;

            if (currentShieldCooldown <= 0f)
            {
                shield.SetActive(false);    
            }
        }

    }

    private void FixedUpdate()
    {
        if (StaticStatus.CurrentHealth > 0)
        {
            HandleMovement();
            StatusAdapter.Instance.TakeDamage(StaticStatus.BleedAmount);
        }
        else if (StaticStatus.EndScreen != null)
        {
            StaticStatus.EndScreen.gameObject.SetActive(true);
            TotalScore();
        }
        if (currentShootCooldown > 0f) currentShootCooldown -= Time.deltaTime;
        if (currentDashCooldown > 0f) currentDashCooldown -= Time.deltaTime;
    }

    public void addScore()
    {
        score++;
        scoreText.text = "" + score;
    }

    public void TotalScore()
    {
        totalScore = score * scoreTi;
        totalScoreText.text = ""+totalScore;
    }

    public void ActivateShield()
    {
        if (currentShieldCooldown <= 0f)
        {
            currentShieldCooldown = shieldCooldownDuration;
            shield.SetActive(true);
        }
    }

    private void SetupReferences()
    {
        if (StaticStatus.Instance == null)
        {
            Debug.LogError("StaticStatus Instance is not initialized. Ensure StaticStatus exists in the scene.");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject.");
        }

        currentMaxSpeed = StaticStatus.MaxSpeed;
    }

    public void SetUpBindings()
    {
        gameplayInputMap = inputAsset.FindActionMap("Player");
        moveAction = gameplayInputMap?.FindAction("Move");
        interactAction = gameplayInputMap?.FindAction("Interact");
        mouseAction = gameplayInputMap?.FindAction("Mouse");
        lookAction = gameplayInputMap?.FindAction("Look");
        attackAction = gameplayInputMap?.FindAction("Attack");
        dashAction = gameplayInputMap?.FindAction("Jump");
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (currentShootCooldown <= 0f && StaticStatus.CurrentHealth > bulletPrefab.GetComponent<BulletBehavior>().EnergyCost())
        {
            SpawnBullet();
        }
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("dash pressed");
        if (currentDashCooldown <= 0f)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Debug.Log("dashPlayed");
        currentDashCooldown = StaticStatus.DashCooldown;
        float dashTime = StaticStatus.DashDuration;
        currentMaxSpeed = StaticStatus.MaxSpeedDash;
        rb.linearVelocity = moveDirection * StaticStatus.DashForce;
        yield return new WaitForSeconds(dashTime);

        currentMaxSpeed = StaticStatus.MaxSpeed;
    }

    private void SpawnBullet()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogWarning("Bullet prefab or spawn point is not set!");
            return;
        }

        float angle = Mathf.Atan2(-targetPlayerRotation.x, targetPlayerRotation.y) * Mathf.Rad2Deg;

        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletRotation);

        Debug.Log($"Bullet Rotation: {bulletRotation.eulerAngles}");

        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            StatusAdapter.Instance.TakeDamage(bulletBehavior.EnergyCost());
            bulletBehavior.Attack(StaticStatus.BulletSpeedMultiplier, gameObject);
            currentShootCooldown = bulletBehavior.GetCooldown(StaticStatus.BulletCooldownMultiplier);
        }
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();
            bool hasValidController = false;

            foreach (var controllerName in controllers)
            {
                if (!string.IsNullOrEmpty(controllerName))
                {
                    hasValidController = true;
                    break;
                }
            }

            if (!controllerConnected && hasValidController)
            {
                controllerConnected = true;
                Debug.Log("Controller Connected");
            }
            else if (controllerConnected && !hasValidController)
            {
                controllerConnected = false;
                Debug.Log("Controller Disconnected");
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void HandleMovement()
    {
        moveDirection = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;

        if (moveDirection != Vector2.zero)
        {
            animator.SetBool("IsRunning", true);
            if (moveDirection.x > 0)
                transform.localScale = new Vector3(1f, 1f, 1);
            else if (moveDirection.x < 0)
                transform.localScale = new Vector3(-1f, 1f, 1);

            rb.AddForce(moveDirection * StaticStatus.AccelerationForce, ForceMode2D.Force);

            if (rb.linearVelocity.magnitude > currentMaxSpeed)
            {
                rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, currentMaxSpeed);
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
            if (rb.linearVelocity.magnitude > 0)
            {
                float decelerationAmount = StaticStatus.DecelerationRate * Time.fixedDeltaTime;
                rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity - rb.linearVelocity.normalized * decelerationAmount, rb.linearVelocity.magnitude);
            }
        }
    }

    public void Damage(float damageAmount)
    {
        StatusAdapter.Instance.TakeDamage(damageAmount);
    }
}