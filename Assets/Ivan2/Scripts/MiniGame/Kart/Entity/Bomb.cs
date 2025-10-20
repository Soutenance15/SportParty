using UnityEngine;

public class Bomb : MonoBehaviour
{
    public enum State
    {
        Immobile,
        Move,
    }

    public float speed = 12f;
    private float lifetime = 3f; // durée avant disparition

    public State state = State.Immobile;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // public void SetSpeed(float speed)
    // {
    //     this.speed = speed;
    // }

    void Update()
    {
        // avance dans la direction "up" locale si state move
        if (state == State.Move)
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }
    }
}
