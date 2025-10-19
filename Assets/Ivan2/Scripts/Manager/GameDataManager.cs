using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameDataManager
{
    public const string P1_KEY = "Current_P1";
    public const string P2_KEY = "Current_P2";
    public const string TOTAL_SUFFIX = "_TotalScore";

    public static string Player1 => PlayerPrefs.GetString(P1_KEY, "Joueur 1");
    public static string Player2 => PlayerPrefs.GetString(P2_KEY, "Joueur 2");

    // Associe chaque manette à son joueur
    // Attention Code ChatGPT
    private static Dictionary<int, string> deviceToPlayer = new Dictionary<int, string>();
    // Attention Code ChatGPT

    public static void SavePlayers(string player1, string player2)
    {
        PlayerPrefs.SetString(P1_KEY, player1);
        PlayerPrefs.SetString(P2_KEY, player2);
        PlayerPrefs.Save();
    }

    public static void AddScore(string playerName, int points)
    {
        int total = PlayerPrefs.GetInt(playerName + TOTAL_SUFFIX, 0);
        PlayerPrefs.SetInt(playerName + TOTAL_SUFFIX, total + points);
        PlayerPrefs.Save();
    }

    public static int GetTotalScore(string playerName)
    {
        return PlayerPrefs.GetInt(playerName + TOTAL_SUFFIX, 0);
    }

    // Nouvelle méthode pour garder le lien manette <-> joueur
    // Attention Code ChatGPT
    public static string GetOrAssignPlayerName(InputDevice device)
    {
        int id = device.deviceId;

        if (deviceToPlayer.TryGetValue(id, out string existingName))
            return existingName;

        // S'il n'existe pas encore, assigne un nom selon le nombre déjà enregistrés
        string playerName = $"Joueur {deviceToPlayer.Count + 1}";
        deviceToPlayer[id] = playerName;
        return playerName;
    }
    // Attention Code ChatGPT
}
