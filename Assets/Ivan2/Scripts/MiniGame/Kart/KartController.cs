using UnityEngine;

public class KartController : MonoBehaviour
{
    // System
    public KartDriveSystem kartDrive;
    private KartAttackSystem kartAttack;
    public KartInputSystem kartInput;
    public NextStepManager nextStepManager;

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
    public int index;

    void Awake()
    {
        kartDrive = GetComponent<KartDriveSystem>();
        kartAttack = GetComponent<KartAttackSystem>();
        kartInput = GetComponent<KartInputSystem>();
        nextStepManager = GetComponent<NextStepManager>();

        if (null != nextStepManager)
        {
            AssignUI();
        }

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

    public void SpawnAtPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void ToggleSkin()
    {
        skin1.enabled = !currentSkinIs1;
        skin2.enabled = currentSkinIs1;
        currentSkinIs1 = !currentSkinIs1;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void AssignUI()
    {
        if (null != nextStepManager)
        {
            nextStepManager.AssignUI(this.index);
        }
    }

    // public void Reset()
    // {
    //     if (null != rb)
    //     {
    //         // Vitesse
    //         rb.linearVelocity = Vector2.zero;
    //         rb.angularVelocity = 0f;

    //         // Orientation
    //         rb.rotation = 0f;
    //         transform.rotation = Quaternion.identity;
    //         transform.position = startPosition;
    //     }
    // }

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
        if (kartInput.NextStepPressed && !isToggleSkinDeactived)
        {
            nextStepManager.NextStep();
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
