using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère automatiquement l’attribution des manettes et du clavier pour le multijoueur local.
/// Permet de jouer avec 1 ou 2 manettes, ou clavier + manette.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class AssignPlayerDevice : MonoBehaviour
{
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.neverAutoSwitchControlSchemes = true;

        var gamepads = Gamepad.all;
        int playerIndex = playerInput.playerIndex;

        // 🧠 Cas 1 — Aucune manette (clavier uniquement)
        if (gamepads.Count == 0)
        {
            if (playerIndex == 0)
            {
                playerInput.SwitchCurrentControlScheme(Keyboard.current);
                Debug.Log("⌨️ Joueur 1 utilise le clavier.");
            }
            else
            {
                Debug.LogWarning("⚠️ Joueur 2 n’a pas de manette ni clavier !");
                gameObject.SetActive(false); // désactive le joueur 2 si aucun input
            }
            return;
        }

        // 🎮 Cas 2 — Une seule manette branchée
        if (gamepads.Count == 1)
        {
            if (playerIndex == 0)
            {
                // Joueur 1 = clavier
                playerInput.SwitchCurrentControlScheme(Keyboard.current);
                Debug.Log("⌨️ Joueur 1 utilise le clavier.");
            }
            else
            {
                // Joueur 2 = manette
                playerInput.SwitchCurrentControlScheme(gamepads[0]);
                Debug.Log($"🎮 Joueur 2 utilise {gamepads[0].displayName}.");
            }
            return;
        }

        // 🎮🎮 Cas 3 — Deux manettes branchées
        if (gamepads.Count >= 2)
        {
            int index = Mathf.Clamp(playerIndex, 0, gamepads.Count - 1);
            playerInput.SwitchCurrentControlScheme(gamepads[index]);
            Debug.Log($"🎮 Joueur {playerIndex + 1} utilise {gamepads[index].displayName}.");
        }
    }
}
