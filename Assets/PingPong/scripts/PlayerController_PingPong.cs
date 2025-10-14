using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_PingPong : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    [Tooltip("Coche si c’est le joueur gauche (sinon joueur droit)")]
    public bool isLeftPlayer = true;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("⚠️ Rigidbody2D manquant sur le paddle !");
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        // Mouvement normal
        Vector2 movement = moveInput.normalized * speed * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + movement;

        // Limites de terrain selon le joueur
        if (isLeftPlayer)
            newPos.x = Mathf.Clamp(newPos.x, -7f, -0.5f);
        else
            newPos.x = Mathf.Clamp(newPos.x, 0.5f, 7f);

        newPos.y = Mathf.Clamp(newPos.y, -4.5f, 4.5f);

        // Applique la position
        rb.MovePosition(newPos);
    }
}
