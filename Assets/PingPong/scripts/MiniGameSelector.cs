using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MiniGameSelector : MonoBehaviour
{
    private static List<string> remainingGames = new List<string>()
    {
        "MiniGame_PingPong",
        "MiniGame_Foot",
        "MiniGame_Basket",
        "MiniGame_Skate"
    };

    void Start()
    {
        if (remainingGames.Count == 0)
        {
            SceneManager.LoadScene("Leaderboard");
            return;
        }

        int index = Random.Range(0, remainingGames.Count);
        string chosenGame = remainingGames[index];

        remainingGames.RemoveAt(index);
        Debug.Log($"🎯 Mini-jeu sélectionné : {chosenGame}");

        SceneManager.LoadScene(chosenGame);
    }
}
