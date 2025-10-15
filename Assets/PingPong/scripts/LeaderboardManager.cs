using UnityEngine;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public TMP_Text leaderboardText;

    void Start()
    {
        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        int champP1 = GameDataManager.GetChampPoints(p1);
        int champP2 = GameDataManager.GetChampPoints(p2);

        leaderboardText.text = "🏆 CLASSEMENT FINAL 🏆\n\n";
        leaderboardText.text += $"{p1} : {champP1} pts\n";
        leaderboardText.text += $"{p2} : {champP2} pts\n\n";

        if (champP1 > champP2)
            leaderboardText.text += $"⭐ Vainqueur : {p1} ⭐";
        else if (champP2 > champP1)
            leaderboardText.text += $"⭐ Vainqueur : {p2} ⭐";
        else
            leaderboardText.text += "🤝 Égalité parfaite !";
    }
}
