using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KartMiniGame : MonoBehaviour, MiniGame
{
    public string Name => "Kart";
    public List<PlayerParty> Players { get; set; }
    public bool HasWinner { get; private set; }
    public event Action OnWinMiniGame;

    public void Start()
    {
        Debug.Log("KartMiniGame Start");
        LoadScene("KartingScene");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Update()
    {
        // Simule la victoire du joueur 1
        if (Input.GetKeyDown(KeyCode.Y))
        {
            HasWinner = true;
            Debug.Log("🏁 Joueur 1 gagne la course !");
        }
    }

    public void InitPlayers(List<PlayerParty> players)
    {
        Players = players;
        Debug.Log("🏎️ Init des players du mini-jeu Kart ! KartMiniGame Interface");
    }

    public void WinMiniGame()
    {
        OnWinMiniGame?.Invoke();
    }
}
