using UnityEngine;
using UnityEngine.InputSystem;

public class FootController : MonoBehaviour
{
    private Rigidbody2D rb;
    public PlayerInput inputActions;

    public Vector2 moveInput;

    [SerializeField] private float speed = 50;


    void Start()
    {
         rb = GetComponent<Rigidbody2D>();

        inputActions = GetComponent<PlayerInput>();
    }

    
    public void OnMove(InputAction.CallbackContext context)
  {
    Debug.Log("Move");

    moveInput = context.ReadValue<Vector2>();

  }

    // Update is called once per frame
    void FixedUpdate()
    {
         rb.linearVelocity = new Vector2(moveInput.x * speed * 100f * Time.fixedDeltaTime, rb.linearVelocity.y);
    }
}
