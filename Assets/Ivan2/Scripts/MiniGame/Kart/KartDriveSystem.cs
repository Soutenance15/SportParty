using UnityEngine;

public class KartDriveSystem : MonoBehaviour
{
    // Moving
    private float acceleration = 10f;
    private float maxSpeed = 15f;
    private float turnSpeed = 180f;
    private float deceleration = 4f;
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
}
