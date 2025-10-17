using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Lance le mode Championnat
    public void OnStartChampionat()
    {
        SceneManager.LoadScene("PlayerSelectMenu");
    }

    // Lance le mode Duel
    public void OnStartDuel()
    {
        SceneManager.LoadScene("DuelGameSelect");
    }

    // Ouvre le menu Options
    public void OnOpenOptions()
    {
        // À créer plus tard
        Debug.Log("Options à venir !");
    }

    // Ouvre la scène des Crédits
    public void OnOpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    // Quitte le jeu
    public void OnQuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}
