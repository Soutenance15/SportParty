// using UnityEngine;

// public class KartInputSystem : MonoBehaviour
// {
//     public static KartInputSystem Instance { get; private set; }

//     public float Vertical { get; private set; }
//     public float Horizontal { get; private set; }
//     public bool FirePressed { get; private set; }

//     void Awake()
//     {
//         // if (Instance != null && Instance != this)
//         // {
//         //     Destroy(gameObject);
//         //     return;
//         // }
//         // Instance = this;
//         // DontDestroyOnLoad(gameObject);
//     }

//     void Update()
//     {
//         // For Drive
//         Vertical = Input.GetAxis("Vertical");
//         Horizontal = Input.GetAxis("Horizontal");

//         // For Attack
//         FirePressed = Input.GetKeyDown(KeyCode.Space);
//     }
// }
