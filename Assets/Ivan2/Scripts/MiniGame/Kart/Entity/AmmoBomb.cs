using System;
using UnityEngine;

public class AmmoBomb : MonoBehaviour
{
    public static Action<KartAttackSystem> OnAmmoEnter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KartAttackSystem kartAttack = collision.gameObject.GetComponent<KartAttackSystem>();
            if (null != kartAttack)
            {
                OnAmmoEnter?.Invoke(kartAttack);
            }
        }
    }
}
