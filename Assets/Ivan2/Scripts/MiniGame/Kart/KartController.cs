using UnityEngine;

public class KartController : MonoBehaviour
{
    // System
    private KartDriveSystem kartDrive;
    private KartAttackSystem kartAttack;

    // Component
    private Rigidbody2D rb;
    public Transform firePoint;

    void Awake()
    {
        kartDrive = GetComponent<KartDriveSystem>();
        kartAttack = GetComponent<KartAttackSystem>();
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
}
