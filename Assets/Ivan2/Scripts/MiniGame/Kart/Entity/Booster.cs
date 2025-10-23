using System;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public static Action<KartDriveSystem> OnBoosterEnter;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KartController"))
        {
            KartDriveSystem kartDriveSystem = collision.gameObject.GetComponent<KartDriveSystem>();
            if (null != kartDriveSystem)
            {
                OnBoosterEnter?.Invoke(kartDriveSystem);
            }
        }
    }
}
