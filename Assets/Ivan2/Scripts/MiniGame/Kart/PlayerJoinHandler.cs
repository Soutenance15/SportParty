using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinHandler : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log($"✅ Nouveau joueur rejoint : {player.name}");
        var kart = player.GetComponent<KartInputSystem>();
        if (kart != null)
            Debug.Log("KartInputSystem détecté sur le joueur.");
    }
}
