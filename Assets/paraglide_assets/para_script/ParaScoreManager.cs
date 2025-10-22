using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ParaScoreManager : MonoBehaviour
{
    public static ParaScoreManager Instance;

    [Header("UI du Score")]
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    [Header("UI du Chronomètre")]
    public float gameDuration = 90f;
    public TextMeshProUGUI timerText;

    [Header("UI de Fin de Partie")]
    public GameObject endGamePanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI loserText;
    public float delayBeforeReturn = 5f;

    private float currentTime;
    private bool gameIsOver = false;
    private string player1Name;
    private string player2Name;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = gameDuration;

        // On récupère les noms des joueurs
        player1Name = GameDataManager.Player1;
        player2Name = GameDataManager.Player2;

        // On met à jour l'affichage initial avec les bons noms
        if (scoreTextP1 != null) scoreTextP1.text = player1Name + " Score: 0";
        if (scoreTextP2 != null) scoreTextP2.text = player2Name + " Score: 0";
    }

    void Update()
    {
        if (!gameIsOver)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame();
            }

            if (timerText != null)
            {
                int minutes = (int)currentTime / 60;
                int seconds = (int)currentTime % 60;
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    void EndGame()
    {
        gameIsOver = true;
        
        var ringSpawner = FindFirstObjectByType<ParaRingSpawner>();
        if (ringSpawner != null) ringSpawner.enabled = false;

        ParaPlayerController[] players = FindObjectsByType<ParaPlayerController>(FindObjectsSortMode.None);
        foreach (var player in players) {
            player.enabled = false;
        }

        int scoreP1 = -1, scoreP2 = -1;
        foreach (var player in players) {
            if (player.playerID == 1) scoreP1 = player.Score;
            if (player.playerID == 2) scoreP2 = player.Score;
        }
        
        GameDataManager.AddScore(player1Name, scoreP1);
        GameDataManager.AddScore(player2Name, scoreP2);
        
        if (scoreP1 > scoreP2) {
            winnerText.text = "Gagnant : " + player1Name + " (+" + scoreP1 + " points)";
            loserText.text = player2Name + " (+" + scoreP2 + " points)";
            GameDataManager.AddChampPoints(player1Name, 3);
            GameDataManager.AddChampPoints(player2Name, 1);
        } else if (scoreP2 > scoreP1) {
            winnerText.text = "Gagnant : " + player2Name + " (+" + scoreP2 + " points)";
            loserText.text = player1Name + " (+" + scoreP1 + " points)";
            GameDataManager.AddChampPoints(player2Name, 3);
            GameDataManager.AddChampPoints(player1Name, 1);
        } else {
            winnerText.text = "Égalité ! (+" + scoreP1 + " points)";
            loserText.text = "";
            GameDataManager.AddChampPoints(player1Name, 2);
            GameDataManager.AddChampPoints(player2Name, 2);
        }
        
        endGamePanel.SetActive(true);
        StartCoroutine(ReturnToMenuCoroutine());
    }

    IEnumerator ReturnToMenuCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeReturn);
        SceneManager.LoadScene("MiniGameSelector");
    }

    // LA CORRECTION EST ICI
    public void UpdateScoreUI(int playerID, int newScore)
    {
        if (playerID == 1)
        {
            if(scoreTextP1 != null) scoreTextP1.text = player1Name + " Score: " + newScore;
        }
        else if (playerID == 2)
        {
            if(scoreTextP2 != null) scoreTextP2.text = player2Name + " Score: " + newScore;
        }
    }
}