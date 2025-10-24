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
    [Tooltip("Délai en secondes avant de charger la scène suivante")]
    public float endDelay = 5f; // Renommé pour correspondre

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
        player1Name = GameDataManager.Player1;
        player2Name = GameDataManager.Player2;
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
        
        // --- Déterminer le Gagnant et Mettre à Jour les Textes ---
        if (winnerText != null && loserText != null) // Vérifie que les textes sont bien assignés
        {
            if (scoreP1 > scoreP2) {
                winnerText.text = "Gagnant : " + player1Name + " (" + scoreP1 + " points)";
                loserText.text = player2Name + " (" + scoreP2 + " points)";
                GameDataManager.AddChampPoints(player1Name, 3);
                GameDataManager.AddChampPoints(player2Name, 1);
            } else if (scoreP2 > scoreP1) {
                winnerText.text = "Gagnant : " + player2Name + " (" + scoreP2 + " points)";
                loserText.text = player1Name + " (" + scoreP1 + " points)";
                GameDataManager.AddChampPoints(player2Name, 3);
                GameDataManager.AddChampPoints(player1Name, 1);
            } else { // Égalité
                winnerText.text = "Égalité ! (" + scoreP1 + " points)";
                // On peut choisir de masquer le texte du perdant ou afficher les deux scores
                loserText.text = player1Name + ": " + scoreP1 + " | " + player2Name + ": " + scoreP2; 
                GameDataManager.AddChampPoints(player1Name, 2);
                GameDataManager.AddChampPoints(player2Name, 2);
            }
        } else {
             Debug.LogError("Les TextMeshProUGUI pour winnerText ou loserText ne sont pas assignés dans l'Inspector !");
        }
        // --- Fin de la Logique Gagnant ---
        
        endGamePanel.SetActive(true);

        // On lance la NOUVELLE coroutine de fin
        StartCoroutine(EndSequence()); // <<< MODIFIÉ
    }

    // NOUVELLE COROUTINE (remplace ReturnToMenuCoroutine)
    IEnumerator EndSequence()
    {
        // Optionnel : Intégration de l'AudioFader (si vous l'avez dans votre projet)
        // if (AudioFader.Instance != null)
        //     AudioFader.Instance.FadeOut(1.5f);

        yield return new WaitForSeconds(endDelay);

        // On récupère le mode de jeu actuel
        string mode = GameDataManager.GetGameMode(); // Assurez-vous que cette fonction existe

        if (mode == "Duel")
        {
            Debug.Log(" Fin de duel – retour vers DuelGameSelect");
            SceneManager.LoadScene("DuelGameSelect");
        }
        else
        {
            Debug.Log(" Fin de manche – retour vers MiniGameSelector");
            SceneManager.LoadScene("MiniGameSelector");
        }
    }

    public void UpdateScoreUI(int playerID, int newScore)
    {
        if (playerID == 1)
        {
            if (scoreTextP1 != null) scoreTextP1.text = player1Name + " Score: " + newScore;
        }
        else if (playerID == 2)
        {
            if (scoreTextP2 != null) scoreTextP2.text = player2Name + " Score: " + newScore;
        }
    }
    // Nouvelle fonction pour obtenir la progression du jeu
    public float GetGameProgress()
    {
        if (gameDuration <= 0) return 0; // Évite la division par zéro
        // Calcule le pourcentage de temps restant (de 1 à 0)
        float progress = currentTime / gameDuration;
        // On inverse pour avoir une progression de 0 (début) à 1 (fin)
        return 1f - Mathf.Clamp01(progress);
    }
}