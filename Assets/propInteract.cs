using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class propInteract : MonoBehaviour
{
    public GameObject interactImage;
    public InputActionReference interactAction;
    public bool isPlayerNerby;
    public UnityEvent onInteract;
    private void Start()
    {
        interactImage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactImage.SetActive(true);
            isPlayerNerby = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactImage.SetActive(false);
            isPlayerNerby = false;
        }
    }

    private void Update()
    {
        if (isPlayerNerby && interactAction.action.WasPressedThisFrame())
        {
            Interact();
        }
    }

    public void Interact()
    {
        onInteract.Invoke();
    }
}
