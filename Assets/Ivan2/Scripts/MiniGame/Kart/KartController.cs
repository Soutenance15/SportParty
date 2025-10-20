using UnityEngine;

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
    public bool isToggleSkinDeactived;
    public bool hasFinished;

    private SpriteRenderer skin1;
    private SpriteRenderer skin2;

    private bool currentSkinIs1;

    // Caracteristique

    public string playerName;

    void Awake()
    {
        kartDrive = GetComponent<KartDriveSystem>();
        kartAttack = GetComponent<KartAttackSystem>();
        kartInput = GetComponent<KartInputSystem>();

        rb = GetComponent<Rigidbody2D>();

        skin1 = transform.Find("Body").Find("Skin1").GetComponent<SpriteRenderer>();
        skin2 = transform.Find("Body").Find("Skin2").GetComponent<SpriteRenderer>();
        skin1.enabled = true;
        skin2.enabled = false;
        currentSkinIs1 = true;

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

    public void ToggleSkin()
    {
        skin1.enabled = !currentSkinIs1;
        skin2.enabled = currentSkinIs1;
        currentSkinIs1 = !currentSkinIs1;
    }

    void Update()
    {
        // Lancer bombe
        if (null != kartInput)
        {
            if (kartInput.FireBombPressed && isActive)
            {
                kartAttack.FireBomb();
            }
        }

        if (kartInput.ChangeSkinPressed && !isToggleSkinDeactived)
        {
            ToggleSkin();
        }
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
