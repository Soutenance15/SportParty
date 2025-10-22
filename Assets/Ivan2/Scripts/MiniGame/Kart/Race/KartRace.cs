using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KartRace : MonoBehaviour
{
    List<KartController> kartsController = new List<KartController>();
    KartController kartControllerWinner;
    bool countDownFinished;
    bool countDownStarted;
    bool isReady;

    GameObject splitUI;
    TextMeshProUGUI countDownText;
    TextMeshProUGUI winningText;

    public float endDelay = 1f;

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
        splitUI = GameObject.Find("SplitUI");
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
        if (kartsController.Count == 2 && !countDownFinished)
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
        if (null != splitUI)
        {
            splitUI.SetActive(false);
        }
        countDownText.enabled = true;
        kartsController[0].isToggleSkinDeactived = true;
        kartsController[1].isToggleSkinDeactived = true;

        kartsController[0].isActive = false;
        kartsController[1].isActive = false;

        kartsController[0].InitAll();
        kartsController[1].InitAll();

        // Replace at good position

        // kartsController[0].kartDrive.ResetAll();
        // kartsController[1].kartDrive.ResetAll();

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

    public void EndMiniGame()
    {
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        if (kartControllerWinner == kartsController[0])
        {
            GameDataManager.AddChampPoints(p1, 25);
        }
        else if (kartControllerWinner == kartsController[1])
        {
            GameDataManager.AddChampPoints(p2, 25);
        }
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        if (AudioFader.Instance != null)
            AudioFader.Instance.FadeOut(1.5f);

        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene("MiniGameSelector");
    }
}
