using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartGameManager : MonoBehaviour
{
    bool gameIsReady;

    int nbKartForStart = 2;
    List<KartController> kartsController = new List<KartController>();
    KartRace kartRace;

    void Awake()
    {
        kartRace = GetComponent<KartRace>();
    }

    void StartGame()
    {
        kartRace.InitGame(kartsController);
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        KartController kartController = player.GetComponent<KartController>();
        if (null != kartController && (kartsController.Count < nbKartForStart))
        {
            kartController.isActive = true;
            GiveNameToPlayer(player, kartController);
            kartsController.Add(kartController);
            kartController.SetIndex(player.playerIndex);
            if (kartsController.Count == 2)
            {
                kartsController[1].SpawnAtPosition(new Vector2(6, 0));
            }
            KartInputSystem.OnStartGame += StartGame;
        }
    }

    // Doit respecter la nomenclature pour le player name
    // A voir avec Daniel
    // Attention Code ChatGPT
    void GiveNameToPlayer(PlayerInput player, KartController kartController)
    {
        // if (player.devices.Count > 0)
        // {
        //     var device = player.devices[0];
        //     kartController.playerName = GameDataManager.GetOrAssignPlayerName(device);
        // }
        // else
        // {
        // fallback au cas où
        kartController.playerName = "Joueur " + (player.playerIndex + 1);
        // }
        // Attention Code ChatGPT
    }
}
