using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager_PingPong : MonoBehaviour
{
    public static GameManager_PingPong Instance;

    [Header("UI")]
    public TMP_Text scoreLeftText;
    public TMP_Text scoreRightText;
    public TMP_Text startInfoText;
    public TMP_Text matchInfoText;

    [Header("References")]
    public BallController ball;
    public EnergyWall energyWallLeft;
    public EnergyWall energyWallRight;

    [Header("Game Settings")]
    public int scoreToWin = 10;
    public float endDelay = 1f;

    private int scoreLeft = 0;
    private int scoreRight = 0;
    private bool matchEnded = false;

    private Coroutine pulseRoutine;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartMatchSequence());
    }

    IEnumerator StartMatchSequence()
    {
        ball.gameObject.SetActive(false);
        if (startInfoText != null)
            startInfoText.text = "Préparation du service...";

        yield return new WaitForSeconds(1f);

        bool leftStarts = Random.value > 0.5f;

        if (startInfoText != null)
        {
            if (leftStarts)
                startInfoText.text = "Joueur Gauche sert →";
            else
                startInfoText.text = "Joueur Droit sert ←";
        }

        if (leftStarts && energyWallLeft != null)
            energyWallLeft.BurstColor();
        else if (!leftStarts && energyWallRight != null)
            energyWallRight.BurstColor();

        yield return new WaitForSeconds(1.5f);

        if (startInfoText != null)
            startInfoText.text = "";

        ball.gameObject.SetActive(true);
        ball.LaunchBall(launchRight: !leftStarts);
    }

    public void GoalScored(bool leftPlayerLost)
    {
        if (matchEnded) return;

        if (leftPlayerLost)
        {
            scoreRight++;
            Debug.Log("🎯 Point pour le joueur droit !");
            UpdateUI();
            Invoke(nameof(ResetBallRightToLeft), 1.5f);
        }
        else
        {
            scoreLeft++;
            Debug.Log("🎯 Point pour le joueur gauche !");
            UpdateUI();
            Invoke(nameof(ResetBallLeftToRight), 1.5f);
        }

        CheckMatchPoint();
        CheckEndCondition();
    }

    void ResetBallRightToLeft() => ball.ResetBall(launchRight: false);
    void ResetBallLeftToRight() => ball.ResetBall(launchRight: true);

    void UpdateUI()
    {
        if (scoreLeftText != null) scoreLeftText.text = scoreLeft.ToString();
        if (scoreRightText != null) scoreRightText.text = scoreRight.ToString();
    }

    void CheckMatchPoint()
    {
        if (matchInfoText == null) return;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        if (scoreLeft == scoreToWin - 1 && scoreRight < scoreToWin - 1)
        {
            matchInfoText.text = $"Match Point pour {GameDataManager.Player1} !";
            matchInfoText.color = new Color(0f, 1f, 1f, 1f);
            pulseRoutine = StartCoroutine(PulseMatchPointText());
        }
        else if (scoreRight == scoreToWin - 1 && scoreLeft < scoreToWin - 1)
        {
            matchInfoText.text = $"Match Point pour {GameDataManager.Player2} !";
            matchInfoText.color = new Color(1f, 0.2f, 0.8f, 1f);
            pulseRoutine = StartCoroutine(PulseMatchPointText());
        }
        else
        {
            matchInfoText.text = "";
        }
    }

    IEnumerator PulseMatchPointText()
    {
        Vector3 baseScale = matchInfoText.transform.localScale;
        float scaleBoost = 1.15f;
        float pulseSpeed = 3f;

        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            matchInfoText.transform.localScale = Vector3.Lerp(baseScale, baseScale * scaleBoost, t);
            matchInfoText.alpha = Mathf.Lerp(0.7f, 1f, t);
            yield return null;
        }
    }

    void CheckEndCondition()
    {
        if (scoreLeft >= scoreToWin || scoreRight >= scoreToWin)
        {
            matchEnded = true;

            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }

            EndMiniGame();
        }
    }

    public void EndMiniGame()
    {
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        if (matchInfoText != null)
        {
            if (pulseRoutine != null)
            {
                StopCoroutine(pulseRoutine);
                pulseRoutine = null;
            }

            matchInfoText.transform.localScale = Vector3.one;

            if (scoreLeft > scoreRight)
            {
                GameDataManager.AddChampPoints(p1, 25);
                matchInfoText.text = $"{p1} remporte la manche !";
                matchInfoText.color = new Color(0f, 1f, 1f, 1f);
            }
            else if (scoreRight > scoreLeft)
            {
                GameDataManager.AddChampPoints(p2, 25);
                matchInfoText.text = $"{p2} remporte la manche !";
                matchInfoText.color = new Color(1f, 0.2f, 0.8f, 1f);
            }
            else
            {
                GameDataManager.AddChampPoints(p1, 10);
                GameDataManager.AddChampPoints(p2, 10);
                matchInfoText.text = "Égalité parfaite !";
                matchInfoText.color = Color.yellow;
            }
        }

        Debug.Log($"Fin du mini-jeu : {p1} ({scoreLeft}) - {p2} ({scoreRight})");
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
