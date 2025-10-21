using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KartRace : MonoBehaviour
{
    List<KartController> kartsController = new List<KartController>();
    KartController kartControllerWinner;
    bool countDownFinished;
    bool countDownStarted;

    TextMeshProUGUI countDownText;
    TextMeshProUGUI winningText;

    private void OnEnable()
    {
        KartRaceFinish.OnFinish += SomeoneFinish;
    }

    private void OnDisable()
    {
        KartRaceFinish.OnFinish -= SomeoneFinish;
    }

    void Awake()
    {
        countDownText = GameObject.Find("CountDownText").GetComponent<TextMeshProUGUI>();
        winningText = GameObject.Find("WinningText").GetComponent<TextMeshProUGUI>();
        if (null != winningText)
        {
            winningText.enabled = false;
        }
        if (null != countDownText)
        {
            countDownText.enabled = false;
        }
    }

    public void InitGame(List<KartController> kartsController)
    {
        if (!countDownFinished)
        {
            if (!countDownStarted)
            {
                countDownStarted = true;
                this.kartsController = kartsController;
                if (null != winningText)
                {
                    winningText.enabled = false;
                }
                if (null != countDownText)
                {
                    countDownFinished = true;
                }
                StartGame();
            }
        }
    }

    void StartGame()
    {
        countDownText.enabled = true;
        kartsController[0].isToggleSkinDeactived = true;
        kartsController[1].isToggleSkinDeactived = true;

        kartsController[0].isActive = false;
        kartsController[1].isActive = false;

        kartsController[0].InitAll();
        kartsController[1].InitAll();

        // Replace at good position
        kartsController[0].SpawnAtPosition(Vector2.zero);
        kartsController[1].SpawnAtPosition(new Vector2(6, 0));

        StartCoroutine(StartCountDown(3f));
        countDownFinished = true;
    }

    private void SomeoneFinish(KartController kartController)
    {
        kartController.hasFinished = true;
        kartController.isActive = false;
        if (null == kartControllerWinner)
        {
            kartControllerWinner = kartController;
        }
        winningText.text = kartControllerWinner.playerName + " a gagné la COURSE !!!";
        winningText.enabled = true;
        if (CheckedAllKartFinished())
        {
            EndGame();
        }
    }

    bool CheckedAllKartFinished()
    {
        if (kartsController[0].hasFinished && kartsController[1].hasFinished)
            return true;
        return false;
    }

    IEnumerator StartCountDown(float time)
    {
        while (time > 0f)
        {
            countDownText.text = time.ToString("F2"); // 2 chiffres après la virgule
            time -= Time.deltaTime; // décrémente en fonction du temps réel
            yield return null; // attend la frame suivante
        }

        // Active the players (movement allowed)
        kartsController[0].isActive = true;
        kartsController[1].isActive = true;

        countDownText.text = "PARTEZ !!!";
        time = 1.5f;
        while (time > 0f)
        {
            time -= Time.deltaTime; // décrémente en fonction du temps réel
            yield return null; // attend la frame suivante
        }
        countDownText.enabled = false;
    }

    void EndGame()
    {
        GameDataManager.AddScore(kartControllerWinner.playerName, 1);
        Debug.Log("End Game + " + kartControllerWinner.playerName.ToString());
        // LoadScene -> Soit directe prochain minigame, soit menu minigame
        // Ou bieen invoke un event EndGame
    }
}
