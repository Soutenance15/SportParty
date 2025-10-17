using UnityEngine;
using UnityEngine.SceneManagement;

public class DuelGameSelect : MonoBehaviour
{
    // Sélectionne le mini-jeu de Ping Pong
    public void OnSelectPingPong()
    {
        SceneManager.LoadScene("PingPong");
    }

    // Sélectionne le mini-jeu de Karting
    public void OnSelectKart()
    {
        SceneManager.LoadScene("Kartscene");
    }

    // Sélectionne le mini-jeu de Parapente
    public void OnSelectParaglide()
    {
        SceneManager.LoadScene("paraglide");
    }

    // Sélectionne le mini-jeu de Football
    public void OnSelectFoot()
    {
        SceneManager.LoadScene("Foot");
    }

    // Retour au menu principal
    public void OnReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
