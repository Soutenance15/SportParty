using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_Text leaderboardText;

    void Start()
    {
        // Vérifie que le texte est bien assigné
        if (leaderboardText == null)
        {
            Debug.LogError("Aucun TMP_Text assigné au LeaderboardManager !");
            return;
        }

        // Récupération des noms et points depuis GameDataManager
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        int champP1 = GameDataManager.GetChampPoints(p1);
        int champP2 = GameDataManager.GetChampPoints(p2);

        // Construction du texte du classement
        leaderboardText.text = "CLASSEMENT FINAL\n\n";
        leaderboardText.text += $"{p1} : {champP1} pts\n";
        leaderboardText.text += $"{p2} : {champP2} pts\n\n";

        // Détermine le vainqueur
        if (champP1 > champP2)
        {
            leaderboardText.text += $"Vainqueur : <b>{p1}</b>";
        }
        else if (champP2 > champP1)
        {
            leaderboardText.text += $"Vainqueur : <b>{p2}</b>";
        }
        else
        {
            leaderboardText.text += "Égalité parfaite !";
        }

        Debug.Log($"Résultat final : {p1} = {champP1} pts | {p2} = {champP2} pts");
    }

    // Bouton rejouer (facultatif)
    public void OnReplay()
    {
        GameDataManager.ResetAll();
        SceneManager.LoadScene("PlayerSelectMenu");
    }
}
