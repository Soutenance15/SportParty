using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerSelectMenuManager : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_InputField inputPlayer1;
    public TMP_InputField inputPlayer2;

    [Header("Nom de la scène de sélection aléatoire")]
    public string miniGameSelectorScene = "MiniGameSelector";

    private void Start()
    {
        // Pré-remplit les champs avec les pseudos précédents si disponibles
        inputPlayer1.text = GameDataManager.Player1;
        inputPlayer2.text = GameDataManager.Player2;
    }

    public void OnStartGame()
    {
        string p1 = inputPlayer1.text.Trim();
        string p2 = inputPlayer2.text.Trim();

        if (string.IsNullOrEmpty(p1)) p1 = "Joueur 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Joueur 2";

        // Sauvegarde les noms dans PlayerPrefs
        GameDataManager.SavePlayers(p1, p2);

        // Remise à zéro des scores globaux pour une nouvelle partie
        PlayerPrefs.DeleteKey(p1 + GameDataManager.CHAMP_POINTS_SUFFIX);
        PlayerPrefs.DeleteKey(p2 + GameDataManager.CHAMP_POINTS_SUFFIX);
        PlayerPrefs.Save();

        Debug.Log($"Nouvelle partie lancée : {p1} vs {p2}");

        // Transition vers la sélection de mini-jeu
        SceneManager.LoadScene(miniGameSelectorScene);
    }

    // Bouton "Retour Menu" — renvoie au menu principal
    public void OnReturnToMainMenu()
    {
        Debug.Log("Retour au menu principal...");
        SceneManager.LoadScene("MainMenu");
    }
}
