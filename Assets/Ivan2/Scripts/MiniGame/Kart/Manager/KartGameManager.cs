using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartGameManager : MonoBehaviour
{
    bool gameIsReady;

    int nbKartForStart = 2;
    List<KartController> kartsController = new List<KartController>();
    KartRace kartRace;

    KartController kartControllerWinner;

    void Awake()
    {
        kartRace = GetComponent<KartRace>();
    }

    void SpawnPositionStart(KartController kartController, Vector2 position)
    {
        kartController.transform.position = position;
    }

    void StartGame()
    {
        if (!gameIsReady && kartsController.Count == nbKartForStart)
        {
            foreach (var kartController in kartsController)
            {
                kartController.isActive = true;
            }
        }
        if (kartRace != null)
        {
            gameIsReady = true;
            kartRace.StartGame(kartsController);
        }
    }

    private void OnEnable()
    {
        KartRaceFinish.OnFinish += SomeoneFinish;
    }

    private void OnDisable()
    {
        KartRaceFinish.OnFinish -= SomeoneFinish;
    }

    private void SomeoneFinish(KartController kartController)
    {
        kartController.hasFinished = true;
        kartController.StopControl();
        if (null == kartControllerWinner)
        {
            kartControllerWinner = kartController;
        }
        if (CheckedAllKartFinished())
        {
            EndGame();
        }
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        KartController kartController = player.GetComponent<KartController>();
        if (null != kartController && (kartsController.Count < nbKartForStart))
        {
            kartController.isActive = false;
            // kartController.ShowBody(false);
            GiveNameToPlayer(player, kartController);
            kartsController.Add(kartController);
            if (kartsController.Count == 2)
            {
                SpawnPositionStart(kartsController[1], new Vector2(3, 0));
            }
            KartInputSystem.OnStartGame += StartGame;
        }
    }

    bool CheckedAllKartFinished()
    {
        if (kartsController[0].hasFinished && kartsController[1].hasFinished)
            return true;
        return false;
    }

    void EndGame()
    {
        GameDataManager.AddScore(kartControllerWinner.playerName, 1);
        Debug.Log("End Game + " + kartControllerWinner.playerName.ToString());
        // LoadScene -> Soit directe prochain minigame, soit menu minigame
        // Ou bieen invoke un event EndGame
    }

    // Doit respecter la nomenclature pour le player name
    // A voir avec Daniel
    // Attention Code ChatGPT
    void GiveNameToPlayer(PlayerInput player, KartController kartController)
    {
        if (player.devices.Count > 0)
        {
            var device = player.devices[0];
            kartController.playerName = GameDataManager.GetOrAssignPlayerName(device);
        }
        else
        {
            // fallback au cas où
            kartController.playerName = "Joueur " + (player.playerIndex + 1);
        }
        // Attention Code ChatGPT
    }
}
