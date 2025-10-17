using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MiniGameSelector : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text infoText;

    private static List<string> remainingGames = new List<string>()
    {
        "PingPong",
        "Kartscene",
        "paraglide",
        "Foot"
    };

    private ScreenFader screenFader;

    private void Start()
    {
        screenFader = FindObjectOfType<ScreenFader>();
        StartCoroutine(SelectRandomMiniGame());
    }

    private IEnumerator SelectRandomMiniGame()
    {
        if (remainingGames.Count == 0)
        {
            infoText.text = "Tous les mini-jeux ont été joués !";
            yield return new WaitForSeconds(2f);

            if (screenFader != null)
                yield return StartCoroutine(screenFader.FadeOutAndLoad("Leaderboard"));
            else
                SceneManager.LoadScene("Leaderboard");

            yield break;
        }

        infoText.text = "Sélection du mini-jeu...";
        yield return new WaitForSeconds(1.5f);

        int index = Random.Range(0, remainingGames.Count);
        string chosenGame = remainingGames[index];
        remainingGames.RemoveAt(index);

        infoText.text = $"Mini-jeu sélectionné : <b>{chosenGame}</b>";
        Debug.Log($"Mini-jeu sélectionné : {chosenGame}");

        yield return new WaitForSeconds(2f);

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeOutAndLoad(chosenGame));
        else
            SceneManager.LoadScene(chosenGame);
    }
}
