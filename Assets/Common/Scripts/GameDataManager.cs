using UnityEngine;

/// <summary>
/// 💾 Gestion centrale des données globales :
/// - Noms des joueurs
/// - Scores / points de championnat
/// - Mode Duel ou Championnat
/// - Compatibilité totale avec anciens mini-jeux
/// </summary>
public static class GameDataManager
{
    public const string P1_KEY = "Current_P1";
    public const string P2_KEY = "Current_P2";
    public const string TOTAL_SUFFIX = "_TotalScore";
    public const string CHAMP_POINTS_SUFFIX = "_ChampPoints";
    public const string MODE_KEY = "CurrentMode"; // "Championship" ou "Duel"

    // ⚙️ Bloc statique exécuté une fois au chargement du jeu
    static GameDataManager()
    {
        EnsureDefaultPlayers();
        MigrateOldKeysIfNeeded();
        SyncKeysBothWays(); // 🔁 garantit la cohérence entre anciens et nouveaux formats
    }

    // 🩹 S'assure qu'il existe toujours au moins des noms par défaut
    private static void EnsureDefaultPlayers()
    {
        if (!PlayerPrefs.HasKey(P1_KEY)) PlayerPrefs.SetString(P1_KEY, "Joueur 1");
        if (!PlayerPrefs.HasKey(P2_KEY)) PlayerPrefs.SetString(P2_KEY, "Joueur 2");
        if (!PlayerPrefs.HasKey("Player1")) PlayerPrefs.SetString("Player1", "Joueur 1");
        if (!PlayerPrefs.HasKey("Player2")) PlayerPrefs.SetString("Player2", "Joueur 2");
        PlayerPrefs.Save();
    }

    // 🧠 Migration des anciennes clés vers les nouvelles (au cas où)
    private static void MigrateOldKeysIfNeeded()
    {
        if (PlayerPrefs.HasKey("Player1") && !PlayerPrefs.HasKey(P1_KEY))
        {
            string oldP1 = PlayerPrefs.GetString("Player1", "Joueur 1");
            string oldP2 = PlayerPrefs.GetString("Player2", "Joueur 2");
            PlayerPrefs.SetString(P1_KEY, oldP1);
            PlayerPrefs.SetString(P2_KEY, oldP2);
            PlayerPrefs.Save();
            Debug.Log("✅ Migration des anciens noms vers Current_P1 / Current_P2");
        }
    }

    // 🔁 Synchronise toujours les deux systèmes (nouveau ↔ ancien)
    private static void SyncKeysBothWays()
    {
        string currentP1 = PlayerPrefs.GetString(P1_KEY, "Joueur 1");
        string currentP2 = PlayerPrefs.GetString(P2_KEY, "Joueur 2");
        string legacyP1 = PlayerPrefs.GetString("Player1", currentP1);
        string legacyP2 = PlayerPrefs.GetString("Player2", currentP2);

        // si un des deux formats est vide, on complète
        if (string.IsNullOrEmpty(currentP1) && !string.IsNullOrEmpty(legacyP1))
            PlayerPrefs.SetString(P1_KEY, legacyP1);
        if (string.IsNullOrEmpty(currentP2) && !string.IsNullOrEmpty(legacyP2))
            PlayerPrefs.SetString(P2_KEY, legacyP2);

        // et on recopie toujours les valeurs actuelles vers l'ancien système
        PlayerPrefs.SetString("Player1", PlayerPrefs.GetString(P1_KEY));
        PlayerPrefs.SetString("Player2", PlayerPrefs.GetString(P2_KEY));
        PlayerPrefs.Save();
    }

    // --- 👤 Accès public aux noms des joueurs ---
    public static string Player1 => PlayerPrefs.GetString(P1_KEY, "Joueur 1");
    public static string Player2 => PlayerPrefs.GetString(P2_KEY, "Joueur 2");

    public static void SavePlayers(string player1, string player2)
    {
        // Nouveau système
        PlayerPrefs.SetString(P1_KEY, player1);
        PlayerPrefs.SetString(P2_KEY, player2);

        // Ancien système (pour compatibilité Golazo et cie)
        PlayerPrefs.SetString("Player1", player1);
        PlayerPrefs.SetString("Player2", player2);

        PlayerPrefs.Save();

        Debug.Log($"💾 Noms sauvegardés : {player1} / {player2} (synchronisation complète)");
    }

    // --- 📊 Scores globaux ---
    public static void AddScore(string playerName, int points)
    {
        int total = PlayerPrefs.GetInt(playerName + TOTAL_SUFFIX, 0);
        PlayerPrefs.SetInt(playerName + TOTAL_SUFFIX, total + points);
        PlayerPrefs.Save();
    }

    // --- 🏆 Points de championnat (uniquement si mode Championnat) ---
    public static void AddChampPoints(string playerName, int points)
    {
        if (GetGameMode() != "Championship") return;
        int total = PlayerPrefs.GetInt(playerName + CHAMP_POINTS_SUFFIX, 0);
        PlayerPrefs.SetInt(playerName + CHAMP_POINTS_SUFFIX, total + points);
        PlayerPrefs.Save();
    }

    public static int GetChampPoints(string playerName)
    {
        return PlayerPrefs.GetInt(playerName + CHAMP_POINTS_SUFFIX, 0);
    }

    // --- 🎮 Mode Duel / Championnat ---
    public static void SetGameMode(string mode)
    {
        PlayerPrefs.SetString(MODE_KEY, mode);
        PlayerPrefs.Save();
    }

    public static string GetGameMode()
    {
        return PlayerPrefs.GetString(MODE_KEY, "Championship");
    }

    // --- 🧹 Réinitialisation complète ---
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("🧹 Toutes les données du GameDataManager ont été effacées.");
    }
}
