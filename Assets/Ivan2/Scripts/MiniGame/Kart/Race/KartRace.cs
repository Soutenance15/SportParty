using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KartRace : MonoBehaviour
{
    public List<KartController> kartsController = new List<KartController>();
    KartController kartControllerWinner;
    bool countDownFinished;
    bool countDownStarted;
    public bool isPlaying;

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
        foreach (KartController kartController in kartsController)
        {
            if (kartController.kartInput.StartPressed)
            {
                kartController.isReady = true;
                InitKartController(kartController);
            }
        }
        if (kartsController.Count == 2)
        {
            Debug.Log("kartsController.Count == 2");
            if (!countDownStarted)
            {
                Debug.Log("!countDownStarted");
                if (kartsController[0].isReady && kartsController[1].isReady)
                {
                    Debug.Log("kartsController[0].isReady && kartsController[1].isReady");
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
    }

    void InitKartController(KartController kartController)
    {
        Debug.Log("Init : " + kartController.index.ToString());
        kartController.isToggleSkinDeactived = true;
        kartController.isActive = false;
        kartController.InitAll();
        kartController.nextStepManager.isReadyText.text = "Prêt";
        if (kartController.index == 0)
        {
            kartController.SpawnAtPosition(Vector2.zero);
        }
        else if (kartController.index == 1)
        {
            kartController.SpawnAtPosition(new Vector2(6, 0));
        }
    }

    void StartGame()
    {
        if (null != splitUI)
        {
            // splitUI.SetActive(false);
            splitUI
                .transform.Find("UI_1")
                .Find("Tuto_UI")
                .Find("Block")
                .gameObject.SetActive(false);
            splitUI
                .transform.Find("UI_2")
                .Find("Tuto_UI")
                .Find("Block")
                .gameObject.SetActive(false);
            splitUI.transform.Find("TutoUIPannel").gameObject.SetActive(false);
        }
        countDownText.enabled = true;
        StartCoroutine(StartCountDown(3f));
        countDownFinished = true;
        isPlaying = true;
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
        EndMiniGame();
        // if (CheckedAllKartFinished())
        // {
        //     EndGame();
        // }
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

        string mode = GameDataManager.GetGameMode();

        if (mode == "Duel")
        {
            Debug.Log("🔁 Fin de duel – retour vers DuelGameSelect");
            SceneManager.LoadScene("DuelGameSelect");
        }
        else
        {
            Debug.Log("🏆 Fin de manche – retour vers MiniGameSelector");
            SceneManager.LoadScene("MiniGameSelector");
        }
    }
}
