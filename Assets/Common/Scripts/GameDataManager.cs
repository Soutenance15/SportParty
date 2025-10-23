using UnityEngine;

public static class GameDataManager
{
    public const string P1_KEY = "Current_P1";
    public const string P2_KEY = "Current_P2";
    public const string TOTAL_SUFFIX = "_TotalScore";
    public const string CHAMP_POINTS_SUFFIX = "_ChampPoints";
    public const string MODE_KEY = "CurrentMode"; // "Championship" ou "Duel"

    public static string Player1 => PlayerPrefs.GetString(P1_KEY, "Joueur 1");
    public static string Player2 => PlayerPrefs.GetString(P2_KEY, "Joueur 2");

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

    // 🏆 Points de championnat
    public static void AddChampPoints(string playerName, int points)
    {
        // ✅ On n'ajoute des points que si le mode actuel est "Championship"
        if (GetGameMode() != "Championship") return;

        int total = PlayerPrefs.GetInt(playerName + CHAMP_POINTS_SUFFIX, 0);
        PlayerPrefs.SetInt(playerName + CHAMP_POINTS_SUFFIX, total + points);
        PlayerPrefs.Save();
    }

    public static int GetChampPoints(string playerName)
    {
        return PlayerPrefs.GetInt(playerName + CHAMP_POINTS_SUFFIX, 0);
    }

    // 🎮 Mode de jeu courant (Championnat / Duel)
    public static void SetGameMode(string mode)
    {
        PlayerPrefs.SetString(MODE_KEY, mode);
        PlayerPrefs.Save();
    }

    public static string GetGameMode()
    {
        return PlayerPrefs.GetString(MODE_KEY, "Championship");
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
