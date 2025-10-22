using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Golazo : MonoBehaviour
{
    [Header("UI Références")]
    public TextMeshProUGUI player1FootScore;
    public TextMeshProUGUI player2FootScore;
    public TextMeshProUGUI winText;
    public GameObject scorePanel;

    [Header("Paramètres")]
    public int p1Score;
    public int p2Score;
    public int maxScore = 5;          // 🏁 Score limite pour gagner
    public float endDelay = 2f;       // ⏱ Délai avant retour au menu

    private string player1Name;
    private string player2Name;
    private bool gameEnded = false;

    void Start()
    {
        // 🧾 Récupère les noms depuis GameDataManager
        player1Name = GameDataManager.Player1;
        player2Name = GameDataManager.Player2;

        // 🔄 Initialise les scores à zéro
        p1Score = 0;
        p2Score = 0;

        // 🖋 Affiche les noms initiaux dans les UI
        player1FootScore.text = $"{player1Name}: {p1Score}";
        player2FootScore.text = $"{player2Name}: {p2Score}";

        if (scorePanel != null)
            scorePanel.SetActive(false);
    }

    // 🥅 Quand le joueur 1 marque
    public void P1Score()
    {
        if (gameEnded) return;

        FootSoundManager.Play("Applause");
        p1Score++;
        player1FootScore.text = $"{player1Name}: {p1Score}";

        Destroy(GameObject.FindGameObjectWithTag("Ball"));

        if (p1Score >= maxScore)
            EndGame(isPlayer1Winner: true);
    }

    // 🥅 Quand le joueur 2 marque
    public void P2Score()
    {
        if (gameEnded) return;

        FootSoundManager.Play("Applause");
        p2Score++;
        player2FootScore.text = $"{player2Name}: {p2Score}";

        Destroy(GameObject.FindGameObjectWithTag("Ball"));

        if (p2Score >= maxScore)
            EndGame(isPlayer1Winner: false);
    }

    // 🏆 Fin de partie
    private void EndGame(bool isPlayer1Winner)
    {
        if (gameEnded) return;
        gameEnded = true;

        string winner = isPlayer1Winner ? player1Name : player2Name;
        string loser  = isPlayer1Winner ? player2Name : player1Name;

        // ✅ Ajoute les points de championnat via GameDataManager
        GameDataManager.AddChampPoints(winner, 25);
        GameDataManager.AddScore(winner, 0); // facultatif : incrément du score global

        // 🏁 Message de victoire
        winText.text = $"{winner} Wins!";

        // 🎬 Active le panneau et fige le jeu
        Time.timeScale = 0;
        scorePanel.SetActive(true);

        // ⏳ Lance la séquence de fin après un court délai
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // ⏳ Attente en temps réel (indépendant du Time.timeScale)
        yield return new WaitForSecondsRealtime(endDelay);

        Time.timeScale = 1;
        SceneManager.LoadScene("MiniGameSelector");
    }
}
