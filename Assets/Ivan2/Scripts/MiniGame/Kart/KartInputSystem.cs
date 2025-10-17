using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartInputSystem : MonoBehaviour
{
    public float Vertical { get; private set; }
    public float Horizontal { get; private set; }
    public bool FireBombPressed { get; private set; }
    public bool StartPressed { get; private set; }

    private Vector2 moveInput;

    public static event Action OnStartGame;

    // Ces fonctions seront appelées automatiquement par le PlayerInput
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Horizontal = moveInput.x;
        Vertical = moveInput.y;
    }

    public void OnFireBomb(InputValue value)
    {
        if (value.isPressed)
            FireBombPressed = true;
        else
            FireBombPressed = false;
    }

    public void OnStart(InputValue value)
    {
        if (value.isPressed)
        {
            StartPressed = true;
            // Le Systeme l'utilisant saura a tout moment que le bouton est appuyé
            // De plus un evenement via une static methode est envoyé
            // Celà permet à la fois de savoir le start ju doueur spécifique
            // Et si n'importe quel joueur à appuyer sur start 
            OnStartGame?.Invoke();
        }
        else
            StartPressed = false;
    }

    void LateUpdate()
    {
        // Reset du FireBombPressed pour que ce soit un appui "instantané"
        FireBombPressed = false;
    }
}
