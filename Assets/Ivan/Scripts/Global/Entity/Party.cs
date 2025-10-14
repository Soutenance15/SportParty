using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public int numberOfPlayers = 0;

    public MiniGame miniGame;
    public bool isRunning;
    public List<PlayerParty> players = new List<PlayerParty>();

    public void AddPlayer(string username)
    {
        numberOfPlayers += 1;
        int id = numberOfPlayers;
        var player = new PlayerParty(username, id);
        players.Add(player);
    }

    public void Start()
    {
        if (miniGame == null)
        {
            Debug.LogWarning("⚠️ Aucun mini-jeu sélectionné !");
            return;
        }
        Debug.LogWarning("⚠️ START !");
        isRunning = true;
        miniGame.InitPlayers(players);
    }
}
