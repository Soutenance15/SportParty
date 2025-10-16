using UnityEngine;
using UnityEngine.InputSystem;

public class KartInputSystem : MonoBehaviour
{
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public bool FireBombPressed { get; private set; }

    private Vector2 moveInput;

    // Ces fonctions seront appelées automatiquement par le PlayerInput
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Horizontal = moveInput.x;
        Vertical = moveInput.y;
    }

    public void OnFireBomb(InputValue value)
    {
        if (value.isPressed)
            FireBombPressed = true;
        else
            FireBombPressed = false;
    }

    void LateUpdate()
    {
        // Reset du FireBombPressed pour que ce soit un appui "instantané"
        FireBombPressed = false;
    }
}
