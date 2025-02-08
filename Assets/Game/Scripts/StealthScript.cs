using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class StealthScript : MonoBehaviour
{
    public GameObject interactImage;
    public InputActionReference interactAction;
    public Transform teleportDestination;
    public bool isPlayerNearby;
    public UnityEvent onStealth;
    public bool alreadyStealthed = false;
    public float Healammount= 15;
    private PlayerBehavior playerB;


    private void Start()
    {
        interactImage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!alreadyStealthed)
        {
            isPlayerNearby = true;
            interactImage.SetActive(true);  // Show the interact image when the player is nearby
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactImage.SetActive(false);  // Hide the interact image when the player exits the area
        }
    }

    private void Update()
    {
        if (isPlayerNearby && interactAction.action.WasPressedThisFrame()&&!alreadyStealthed)
        {
            Interact();
        }
    }

    public void Interact()
    {
        // Teleport the player when the interact action is triggered
        if (isPlayerNearby)
        {
            GameObject player = GameObject.FindWithTag("Player"); // Find the player object
            if (player != null)
            {
                player.transform.position = teleportDestination.position;
                onStealth.Invoke();
                alreadyStealthed = true; 
                interactImage.SetActive(false);
                StatusAdapter.Instance.RestoreHealth(Healammount);
                playerB.addScore();
            }
        }
    }
}
