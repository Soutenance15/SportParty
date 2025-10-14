using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager_PingPong : MonoBehaviour
{
    public static GameManager_PingPong Instance;

    [Header("UI")]
    public TMP_Text scoreLeftText;
    public TMP_Text scoreRightText;
    public TMP_Text startInfoText; // affichage du pile ou face

    [Header("References")]
    public BallController ball;
    public EnergyWall energyWallLeft;
    public EnergyWall energyWallRight;

    private int scoreLeft = 0;
    private int scoreRight = 0;

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
        ball.LaunchBall(launchRight: !leftStarts); // ✅ la balle part vers l'adversaire
    }

    public void GoalScored(bool leftPlayerLost)
    {
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
    }

    void ResetBallRightToLeft() => ball.ResetBall(launchRight: false);
    void ResetBallLeftToRight() => ball.ResetBall(launchRight: true);

    void UpdateUI()
    {
        if (scoreLeftText != null) scoreLeftText.text = scoreLeft.ToString();
        if (scoreRightText != null) scoreRightText.text = scoreRight.ToString();
    }
}
