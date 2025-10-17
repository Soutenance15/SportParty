using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartGameManager : MonoBehaviour
{
    bool gameIsReady;
    Vector2 position = Vector2.zero;

    int nbKartForStart = 2;
    List<KartController> kartsController = new List<KartController>();
    KartRace kartRace;

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
            foreach (var kart in kartsController)
            {
                var kartController = kart.GetComponent<KartController>();
                kartController.isActive = true;
                kartController.ShowBody(true);
            }
        }
        gameIsReady = true;
        if (kartRace != null)
        {
            kartRace.StartGame(kartsController);
        }
    }

    private void OnEnable()
    {
        KartRaceFinish.OnFinish += Finish;
    }

    private void OnDisable()
    {
        KartRaceFinish.OnFinish -= Finish;
    }

    private void Finish(KartController kartController)
    {
        Debug.Log(" Un kart a gagné");
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        KartController kartController = player.GetComponent<KartController>();
        if (null != kartController && (kartsController.Count < nbKartForStart))
        {
            kartController.isActive = false;
            kartController.ShowBody(false);
            kartsController.Add(kartController);
            KartInputSystem.OnStartGame += StartGame;
        }
    }
}
