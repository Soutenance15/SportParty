using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KartControllerPhysique : MonoBehaviour
{
    [Header("Paramètres de mouvement")]
    public float acceleration = 10f; // Vitesse d'accélération
    public float maxSpeed = 25f; // Vitesse max
    public float rotationSpeed = 50f; // Vitesse de rotation
    public float drag = 2f; // Freinage passif pour plus de contrôle

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Pas de gravité
        rb.drag = drag; // Freinage passif
        rb.angularDrag = 0f;
    }

    void FixedUpdate()
    {
        // Rotation
        float turnInput = Input.GetAxis("Horizontal");
        rb.rotation -= turnInput * rotationSpeed * Time.fixedDeltaTime;

        // Mouvement avant/arrière
        float moveInput = Input.GetAxis("Vertical");
        Vector2 targetVelocity = transform.up * moveInput * maxSpeed;

        // Appliquer acceleration progressive vers la vitesse cible
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );
    }
}
