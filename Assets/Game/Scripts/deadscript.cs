using UnityEngine;
using UnityEngine.Events;

public class deadscript : MonoBehaviour
{
    public UnityEvent OnDead;
    public string LayerName="PlayerBulet";
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer(LayerName))
        {
            OnDead.Invoke();
        }
    }
}
