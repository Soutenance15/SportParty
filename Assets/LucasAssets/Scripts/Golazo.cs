using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Golazo : MonoBehaviour
{
    public TextMeshProUGUI player1FootScore;
    public TextMeshProUGUI player2FootScore;
    public TextMeshProUGUI winText;
    public GameObject scorePanel;

    public int p1Score;
    public int p2Score;

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
        player1FootScore.text = "P1:" + p1Score.ToString();
        Destroy(GameObject.FindGameObjectWithTag("Ball"));
    }
    
     public void P2Score()
    {
        p2Score++;
        player2FootScore.text = "P2:" + p2Score.ToString();
        Destroy(GameObject.FindGameObjectWithTag("Ball"));
    }

    // Update is called once per frame
    void Update()
    {
        if (p1Score >= 5)
        {
            player1FootScore.text = "P1 WINS!";
            Destroy(GameObject.FindGameObjectWithTag("Ball"));
            scorePanel.SetActive(true);
            winText.text = "Player 1 Wins!";


        }
        if (p2Score >= 5)
        {
            player2FootScore.text = "P2 WINS!";
            Destroy(GameObject.FindGameObjectWithTag("Ball"));
            scorePanel.SetActive(true);
            winText.text = "Player 2 Wins!";
        }
    }
}
