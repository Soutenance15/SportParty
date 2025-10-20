using UnityEngine;
using UnityEngine.InputSystem;
public class JumpFoot : MonoBehaviour
{
     public Rigidbody2D rb;
    public PlayerInput playerInput;
    public float jumpForce;
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public bool isGrounded;
    public LayerMask groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void Jumping(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        if (context.performed && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D groundCollider = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position, groundLayer);
        isGrounded = groundCollider != null;
    }
}
