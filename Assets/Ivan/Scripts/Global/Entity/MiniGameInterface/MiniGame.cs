using System;
using System.Collections.Generic;

public interface MiniGame
{
    // ATTENTION en C# les interfaces n'ont pas de variable ordinnaire
    // Il faut preciser entre acollade : get, set en fonction des besoins
    // Get lecture seule, get et set modification possible
    string Name { get; }
    List<PlayerParty> Players { get; set; }
    bool HasWinner { get; }
    public event Action OnWinMiniGame;
    void InitPlayers(List<PlayerParty> players);
    void Update();
    void Start();
    public void WinMiniGame();
}
