using UnityEngine;
using UnityEngine.UIElements;

public class KartGameManager : MonoBehaviour
{
    bool gameIsReady;
    Vector2 position = Vector2.zero;

    int nbKartForStart = 2;

    void Update()
    {
        // Récupère tous les KartController actifs dans la scène (non triés, plus rapide)
        KartController[] allKarts = FindObjectsByType<KartController>(FindObjectsSortMode.None);
        if (!gameIsReady && allKarts.Length > 0)
        {
            foreach (var kart in allKarts)
            {
                var kartController = kart.GetComponent<KartController>();
                if (kartController != null)
                {
                    ShowBody(kartController, false);
                }
                if (kartController.kartInput.StartPressed && allKarts.Length == nbKartForStart)
                {
                    foreach (var kart2 in allKarts)
                    {
                        var kartController2 = kart2.GetComponent<KartController>();
                        ShowBody(kartController2, true);
                        kartController2.InitAll();
                        SpawnPositionStart(kartController2, position);
                    }
                    StartGame();
                }
            }
        }
    }

    void ShowBody(KartController kartController, bool show)
    {
        Transform kartBody = kartController.transform.Find("Body");
        kartBody.gameObject.SetActive(show);
    }

    void SpawnPositionStart(KartController kartController, Vector2 position)
    {
        kartController.transform.position = position;
    }

    void StartGame()
    {
        gameIsReady = true;
    }
}
