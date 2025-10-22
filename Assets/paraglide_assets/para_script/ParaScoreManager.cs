using UnityEngine;
using TMPro;

public class ParaScoreManager : MonoBehaviour
{
    public static ParaScoreManager Instance;

    [Header("UI du Score")]
    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    [Header("UI du Chronomètre")]
    public float gameDuration = 90f; // Durée de la partie en secondes
    public TextMeshProUGUI timerText;

    private float currentTime;
    private bool gameIsOver = false;

    [Header("UI de Fin de Partie")]
    public GameObject endGamePanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI loserText;

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
        // Si la partie est en cours
        if (!gameIsOver)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame(); // La partie est finie !
            }

            // Mise à jour de l'affichage du temps
            int minutes = (int)currentTime / 60;
            int seconds = (int)currentTime % 60;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void EndGame()
    {
        gameIsOver = true;
        
        // On utilise la nouvelle fonction recommandée
        var ringSpawner = FindFirstObjectByType<ParaRingSpawner>(); // <<< LIGNE MODIFIÉE
        if (ringSpawner != null) ringSpawner.enabled = false;

        // On utilise la nouvelle fonction recommandée
        ParaPlayerController[] players = FindObjectsByType<ParaPlayerController>(FindObjectsSortMode.None); // <<< LIGNE MODIFIÉE
        foreach (var player in players) {
            player.enabled = false;
        }

        int scoreP1 = -1, scoreP2 = -1;
        foreach (var player in players) {
            // On utilise la propriété publique "Score" avec une majuscule
            if (player.playerID == 1) scoreP1 = player.Score; // <<< LIGNE MODIFIÉE
            if (player.playerID == 2) scoreP2 = player.Score; // <<< LIGNE MODIFIÉE
        }

        if (scoreP1 > scoreP2) {
            winnerText.text = "Gagnant : Joueur 1 (" + scoreP1 + " points)";
            loserText.text = "Joueur 2 (" + scoreP2 + " points)";
        } else if (scoreP2 > scoreP1) {
            winnerText.text = "Gagnant : Joueur 2 (" + scoreP2 + " points)";
            loserText.text = "Joueur 1 (" + scoreP1 + " points)";
        } else {
            winnerText.text = "Égalité ! (" + scoreP1 + " points)";
            loserText.text = "";
        }
        
        endGamePanel.SetActive(true);
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