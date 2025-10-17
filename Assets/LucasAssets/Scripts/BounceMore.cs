using UnityEngine;

public class BounceMore : MonoBehaviour
{
    public Rigidbody2D rb;
    public float bouncyness;
    public bool isGrounded;

    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public LayerMask groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
     
    }
}
