using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerBehavior PlayerBehavior;
    private Image cursor;
    private Color tempColor;
    void Awake()
    {
        PlayerBehavior = player.GetComponent<PlayerBehavior>();
        cursor = GetComponent<Image>();
    }

    void FixedUpdate()
    {
        if (PlayerBehavior.controllerConnected)
        {
            tempColor.a = 0;

            PlayerBehavior.targetPlayerRotation = PlayerBehavior.lookAction.ReadValue<Vector2>();
        }
        else
        {
            tempColor = Color.white;

            Vector2 mouseScreenPosition = PlayerBehavior.mouseAction.ReadValue<Vector2>();

            transform.position = mouseScreenPosition;

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, Camera.main.nearClipPlane));

            PlayerBehavior.targetPlayerRotation = new Vector2(
                mouseWorldPosition.x - player.transform.position.x,
                mouseWorldPosition.y - player.transform.position.y
            );
        }
        cursor.color = tempColor;
    }
}
