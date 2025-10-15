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
    public TMP_Text startInfoText; // affichage du pile ou face
    public TMP_Text matchInfoText; // affichage du Match Point ou fin

    [Header("References")]
    public BallController ball;
    public EnergyWall energyWallLeft;
    public EnergyWall energyWallRight;

    [Header("Game Settings")]
    public int scoreToWin = 10; // 🎯 nombre de points nécessaires pour gagner
    public float endDelay = 2f; // délai avant la fin du match

    private int scoreLeft = 0;
    private int scoreRight = 0;
    private bool matchEnded = false;

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

        // Effet "pile ou face"
        bool leftStarts = Random.value > 0.5f;

        // Texte plus clair avec flèches directionnelles
        if (startInfoText != null)
        {
            if (leftStarts)
                startInfoText.text = "Joueur Gauche sert →";
            else
                startInfoText.text = "Joueur Droit sert ←";
        }

        // Flash sur le EnergyWall du joueur choisi
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

    // ⚡ Affiche le "Match Point" quand un joueur est à un point de gagner
    void CheckMatchPoint()
    {
        if (matchInfoText == null) return;

        if (scoreLeft == scoreToWin - 1 && scoreRight < scoreToWin - 1)
        {
            matchInfoText.text = $"Match Point pour {GameDataManager.Player1} !";
            StartCoroutine(FlashMatchText());
        }
        else if (scoreRight == scoreToWin - 1 && scoreLeft < scoreToWin - 1)
        {
            matchInfoText.text = $"Match Point pour {GameDataManager.Player2} !";
            StartCoroutine(FlashMatchText());
        }
        else
        {
            matchInfoText.text = "";
        }
    }

    IEnumerator FlashMatchText()
    {
        Color baseColor = matchInfoText.color;
        for (int i = 0; i < 6; i++)
        {
            matchInfoText.enabled = !matchInfoText.enabled;
            yield return new WaitForSeconds(0.3f);
        }
        matchInfoText.enabled = true;
        matchInfoText.color = baseColor;
    }

    // 🏁 Vérifie si la partie est terminée
    void CheckEndCondition()
    {
        if (scoreLeft >= scoreToWin || scoreRight >= scoreToWin)
        {
            matchEnded = true;
            EndMiniGame();
        }
    }

    // 🔚 Termine la partie et enregistre le résultat global
    public void EndMiniGame()
    {
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        if (matchInfoText != null)
            matchInfoText.text = "";

        // 🏆 Attribution des points de championnat (3 pts au gagnant)
        if (scoreLeft > scoreRight)
        {
            GameDataManager.AddChampPoints(p1, 3);
            if (matchInfoText != null)
                matchInfoText.text = $"{p1} remporte la manche !";
        }
        else if (scoreRight > scoreLeft)
        {
            GameDataManager.AddChampPoints(p2, 3);
            if (matchInfoText != null)
                matchInfoText.text = $"{p2} remporte la manche !";
        }
        else
        {
            GameDataManager.AddChampPoints(p1, 1);
            GameDataManager.AddChampPoints(p2, 1);
            if (matchInfoText != null)
                matchInfoText.text = "Égalité parfaite !";
        }

        Debug.Log($"Fin du mini-jeu : {p1} ({scoreLeft}) - {p2} ({scoreRight})");

        // Transition vers la scène suivante (le sélecteur de mini-jeux ou le leaderboard)
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene("MiniGameSelector"); // ou "Leaderboard" selon ta progression actuelle
    }
}
