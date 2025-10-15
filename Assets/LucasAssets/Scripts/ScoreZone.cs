using UnityEngine;
using UnityEngine.Events;

public class ScoreZone : MonoBehaviour
{
    public UnityEvent<GameObject> OnTriggerEnterEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
             OnTriggerEnterEvent?.Invoke(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
