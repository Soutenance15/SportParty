using UnityEngine;
using TMPro;
using System.Collections; // Requis pour les Coroutines
using UnityEngine.SceneManagement; // Requis pour changer de scène

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
    [Tooltip("Délai en secondes avant de retourner au menu")]
    public float delayBeforeReturn = 5f; // <<< NOUVELLE VARIABLE

    private float currentTime;
    private bool gameIsOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTime = gameDuration;
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

            int minutes = (int)currentTime / 60;
            int seconds = (int)currentTime % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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

        string p1Name = GameDataManager.Player1;
        string p2Name = GameDataManager.Player2;
        int scoreP1 = -1, scoreP2 = -1;
        foreach (var player in players) {
            if (player.playerID == 1) scoreP1 = player.Score;
            if (player.playerID == 2) scoreP2 = player.Score;
        }
        
        GameDataManager.AddScore(p1Name, scoreP1);
        GameDataManager.AddScore(p2Name, scoreP2);
        
        if (scoreP1 > scoreP2) {
            winnerText.text = "Gagnant : " + p1Name + " (+" + scoreP1 + " points)";
            loserText.text = p2Name + " (+" + scoreP2 + " points)";
            GameDataManager.AddChampPoints(p1Name, 3);
            GameDataManager.AddChampPoints(p2Name, 1);
        } else if (scoreP2 > scoreP1) {
            winnerText.text = "Gagnant : " + p2Name + " (+" + scoreP2 + " points)";
            loserText.text = p1Name + " (+" + scoreP1 + " points)";
            GameDataManager.AddChampPoints(p2Name, 3);
            GameDataManager.AddChampPoints(p1Name, 1);
        } else {
            winnerText.text = "Égalité ! (+" + scoreP1 + " points)";
            loserText.text = "";
            GameDataManager.AddChampPoints(p1Name, 2);
            GameDataManager.AddChampPoints(p2Name, 2);
        }
        
        endGamePanel.SetActive(true);

        // On lance la coroutine pour retourner au menu
        StartCoroutine(ReturnToMenuCoroutine()); // <<< NOUVELLE LIGNE
    }

    // NOUVELLE FONCTION
    IEnumerator ReturnToMenuCoroutine()
    {
        // On attend le nombre de secondes défini
        yield return new WaitForSeconds(delayBeforeReturn);

        // On charge la scène du sélecteur de mini-jeux
        SceneManager.LoadScene("MiniGameSelector");
    }

    public void UpdateScoreUI(int playerID, int newScore)
    {
        if (playerID == 1)
        {
            scoreTextP1.text = "P1 Score: " + newScore;
        }
        else if (playerID == 2)
        {
            scoreTextP2.text = "P2 Score: " + newScore;
        }
    }
}