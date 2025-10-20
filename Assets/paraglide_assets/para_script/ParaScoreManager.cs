using UnityEngine;
using TMPro; // Important pour manipuler le texte de l'UI

public class ParaScoreManager : MonoBehaviour
{
    // On utilise un "singleton" pour y accéder facilement depuis n'importe où
    public static ParaScoreManager Instance;

    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    void Awake()
    {
        Instance = this;
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