using UnityEngine;
using TMPro;

public class PlayerNameDisplay : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_Text player1NameText;
    public TMP_Text player2NameText;

    private void Start()
    {
        // Récupère les noms sauvegardés
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        // Les affiche à l’écran
        if (player1NameText != null) player1NameText.text = p1;
        if (player2NameText != null) player2NameText.text = p2;
    }
}
