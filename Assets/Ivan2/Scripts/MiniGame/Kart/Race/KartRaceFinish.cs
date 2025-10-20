using System;
using UnityEngine;

public class KartRaceFinish : MonoBehaviour
{
    public static event Action<KartController> OnFinish;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("avant");
        if (collision.gameObject.CompareTag("KartController"))
        {
            Debug.Log("kart controlelr trigger");
            KartController kartController = collision.GetComponent<KartController>();
            if (null != kartController)
            {
                OnFinish?.Invoke(kartController);
            }
        }
    }
}
