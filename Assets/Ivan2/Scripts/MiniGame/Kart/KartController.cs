using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    // System
    private KartDriveSystem kartDrive;
    private KartAttackSystem kartAttack;
    private KartInputSystem kartInput;

    // Component
    private Rigidbody2D rb;
    public Transform firePoint;

    void Awake()
    {
        kartDrive = GetComponent<KartDriveSystem>();
        kartAttack = GetComponent<KartAttackSystem>();
        kartInput = GetComponent<KartInputSystem>();
        rb = GetComponent<Rigidbody2D>();

        // Init
        if (null != rb)
        {
            kartDrive.InitRb(rb);
            kartAttack.InitRb(rb);
        }
        if (null != firePoint)
        {
            kartAttack.InitFireBomb(firePoint);
        }
    }

    void Update()
    {
        // Lancer bombe
        if (null != kartInput)
        {
            if (kartInput.FireBombPressed)
            {
                kartAttack.FireBomb();
            }
        }
    }

    void FixedUpdate()
    {
        if (null != kartInput)
        {
            float moveInput = kartInput.Vertical;
            float turnInput = kartInput.Horizontal;
            kartDrive.Move(moveInput, turnInput);
        }
    }
}
