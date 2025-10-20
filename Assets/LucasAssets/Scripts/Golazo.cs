using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Golazo : MonoBehaviour
{
    public TextMeshProUGUI player1FootScore;
    public TextMeshProUGUI player2FootScore;
    public TextMeshProUGUI winText;
    public GameObject scorePanel;

    public int p1Score;
    public int p2Score;
    public float endDelay = 1f;

    private string player1Name;
    private string player2Name;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p1Score = 0;
        p2Score = 0;
        player1FootScore.text = "P1:" + p1Score.ToString();
        player2FootScore.text = "P2:" + p2Score.ToString();

    }

    public void P1Score()
    {
        p1Score++;
        player1FootScore.text = $"{GameDataManager.Player1}:" + p1Score.ToString();
        Destroy(GameObject.FindGameObjectWithTag("Ball"));
    }

    public void P2Score()
    {
        p2Score++;
        player2FootScore.text = $"{GameDataManager.Player2}" + p2Score.ToString();
        Destroy(GameObject.FindGameObjectWithTag("Ball"));
    }

    private void EndGame(string winner, string loser, bool isPlayer1Winner)
    {
        if (scorePanel.activeSelf) return;

        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        if (isPlayer1Winner)
        {
            winText.text = $"{GameDataManager.Player1} Wins!";
            GameDataManager.AddChampPoints(p1, 25);
        }
        else
        {
            GameDataManager.AddChampPoints(p2, 25);
            winText.text = $"{GameDataManager.Player2} Wins!";
        }

        Time.timeScale = 0;
        scorePanel.SetActive(true);

        StartCoroutine(EndSequence());
    }
    
    IEnumerator EndSequence()
    {
                yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene("MiniGameSelector");
    }

    // Update is called once per frame
    void Update()
    {
        if (p1Score >= 5)
        {
          EndGame(winner: player1Name, loser: player2Name, isPlayer1Winner: true);
        }
        if (p2Score >= 5)
        {
           EndGame(winner: player2Name, loser: player1Name, isPlayer1Winner: false);
        }
    }
}
