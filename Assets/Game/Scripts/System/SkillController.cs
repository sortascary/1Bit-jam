using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap actionMap;
    private InputAction one;
    private InputAction two;
    private InputAction three;
    private InputAction four;

    [SerializeField] float opacity;
    [SerializeField] private Image skill1;
    [SerializeField] private Image skill2;
    [SerializeField] private Image skill3;
    [SerializeField] private Image skill4;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject laserBullet;
    [SerializeField] private GameObject homingBullet;


    PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = player.GetComponent<PlayerBehavior>();
        SetUpBindings();
    }

    public void SetUpBindings()
    {
        actionMap = inputActions.FindActionMap("Player");
        one = actionMap?.FindAction("1");
        two = actionMap?.FindAction("2");
        three = actionMap?.FindAction("3");
        four = actionMap?.FindAction("4");
    }

    private void OnEnable()
    {
        one.Enable();
        two.Enable();
        three.Enable();
        four.Enable();

        one.performed += ctx =>
        {
            if (playerBehavior.bulletPrefab == laserBullet) playerBehavior.bulletPrefab = bullet; else playerBehavior.bulletPrefab = laserBullet;
        };

        two.performed += ctx =>
        {
            if (playerBehavior.bulletPrefab == homingBullet) playerBehavior.bulletPrefab = bullet; else playerBehavior.bulletPrefab = homingBullet;
        };

        four.performed += ctx =>
        {
            // Example: Activate a shield
            playerBehavior.ActivateShield();
        };
    }

    private void OnDisable()
    {
        one.Disable();
        two.Disable();
        three.Disable();
        four.Disable();

        one.performed -= ctx =>
        {

        };

        two.performed -= ctx =>
        {

        };
    }

    private void Update()
    {
        if (playerBehavior.bulletPrefab.name == "LaserBullet")
        {
            skill1.color = new Color(skill1.color.r,skill1.color.g,skill1.color.b, opacity);
            skill2.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, 1);
        }
        else if (playerBehavior.bulletPrefab.name == "Homing")
        {
            skill1.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, 1);
            skill2.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, opacity);
        }
        else
        {
            skill1.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, 1);
            skill2.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, 1);
        }

        if(playerBehavior.currentDashCooldown <=0f) skill3.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, 1); else skill3.color = new Color(skill1.color.r, skill1.color.g, skill1.color.b, opacity);

        if (playerBehavior.currentShieldCooldown <= 0f) skill4.color = new Color(skill4.color.r, skill4.color.g, skill4.color.b, 1); else skill4.color = new Color(skill4.color.r, skill4.color.g, skill4.color.b, opacity);
    }
}
