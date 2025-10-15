using UnityEngine;

public class KartInputSystem : MonoBehaviour
{
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public bool FireBombPressed { get; private set; }

     void Update()
    {
        // For Drive
        Vertical = Input.GetAxis("Vertical");
        Horizontal = Input.GetAxis("Horizontal");

        // For Attack
        FireBombPressed = Input.GetKeyDown(KeyCode.Space);
    }
}
