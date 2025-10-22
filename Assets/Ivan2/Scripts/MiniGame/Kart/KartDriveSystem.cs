using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartDriveSystem : MonoBehaviour
{
    // Moving
    private float acceleration = 12f;
    private float maxSpeed = 40f;
    private float turnSpeed = 90f;
    private float deceleration = 8f;
    private float currentSpeed = 0f; // vitesse positive ou négative

    // Component
    private Rigidbody2D rb;

    public void InitRb(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }

    public void Move(float moveInput, float turnInput)
    {
        // --- Accélération avant/arrière ---
        if (moveInput != 0f)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.5f, maxSpeed); // arrière plus lent
        }
        else
        {
            // --- Décélération quand on lâche ---
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // --- Rotation ---
        rb.rotation -= turnInput * turnSpeed * Time.fixedDeltaTime;

        // --- Appliquer la vitesse en direction du kart ---
        rb.linearVelocity = transform.up * currentSpeed;
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void ResetAll()
    {
        ResetVelocity();
        rb.rotation = 0f;
        rb.transform.rotation = Quaternion.identity;
    }
}
