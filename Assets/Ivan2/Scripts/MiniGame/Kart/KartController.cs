using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    // System
    private KartDriveSystem kartDrive;
    private KartAttackSystem kartAttack;
    public KartInputSystem kartInput;

    // Component
    private Rigidbody2D rb;
    public Transform firePoint;

    public bool isActive;

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

    public void ShowBody(bool show)
    {
        Transform kartBody = transform.Find("Body");
        kartBody.gameObject.SetActive(show);
    }

    void FixedUpdate()
    {
        if (null != kartInput && isActive)
        {
            float moveInput = kartInput.Vertical;
            float turnInput = kartInput.Horizontal;
            kartDrive.Move(moveInput, turnInput);
        }
    }

    public void InitAll()
    {
        if (null != rb)
        {
            kartDrive.InitRb(rb);
            kartAttack.InitRb(rb);
            kartDrive.ResetVelocity();
        }
        if (null != firePoint)
        {
            kartAttack.InitFireBomb(firePoint);
        }
    }
}
