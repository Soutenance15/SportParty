using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniGameSelector : MonoBehaviour
{
    [System.Serializable]
    public class MiniGameSlot
    {
        public string gameName;
        public Image image;
    }

    [Header("UI")]
    public TMP_Text infoText;
    public List<MiniGameSlot> miniGames;
    public Color highlightColor = new Color(1f, 0.85f, 0.2f);
    public Color normalColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Audio")]
    public AudioClip selectSound;
    public AudioClip whooshSound;
    public AudioClip backgroundMusic;
    public float musicVolume = 0.8f;
    public float fadeOutDuration = 1.5f;

    private AudioSource audioSource;
    private AudioSource musicSource;

    [Header("Effet visuel")]
    public Image flashOverlay;

    private static List<string> remainingGames;
    private ScreenFader screenFader;
    private Coroutine selectionRoutine;
    private string lastPlayedGame = "";

    [System.Serializable]
    private class Wrapper { public List<string> games; }

    private void Start()
    {
        screenFader = UnityEngine.Object.FindFirstObjectByType<ScreenFader>();

        // 🔊 Préparation audio
        audioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }

        if (flashOverlay != null)
            flashOverlay.color = new Color(1f, 1f, 1f, 0f);

        // 📂 Chargement de la progression
        LoadRemainingGames();
        lastPlayedGame = PlayerPrefs.GetString("LastPlayedGame", "");

        // 🧹 Retire le jeu précédemment joué
        if (!string.IsNullOrEmpty(lastPlayedGame) && remainingGames.Contains(lastPlayedGame))
        {
            remainingGames.Remove(lastPlayedGame);
            SaveRemainingGames();
            Debug.Log($"🧹 Retrait du jeu terminé : {lastPlayedGame}");
        }

        // 🏁 Si plus de jeux, direction leaderboard
        if (remainingGames == null || remainingGames.Count == 0)
        {
            StartCoroutine(GoToLeaderboard());
            return;
        }

        // ✅ Lancer la sélection
        selectionRoutine = StartCoroutine(SelectMiniGameWithAnimation());
    }

    private IEnumerator SelectMiniGameWithAnimation()
    {
        infoText.text = "Sélection du mini-jeu...";
        yield return new WaitForSeconds(0.8f);

        int highlightIndex = 0;
        float delay = 0.12f;
        int rounds = Random.Range(12, 18);

        for (int i = 0; i < rounds; i++)
        {
            HighlightImage(highlightIndex);

            if (whooshSound != null)
                audioSource.PlayOneShot(whooshSound, 0.7f);

            highlightIndex = (highlightIndex + 1) % miniGames.Count;
            yield return new WaitForSeconds(delay);
            delay *= 1.09f;
        }

        // 🔽 Sélection finale
        int finalIndex = (highlightIndex - 1 + miniGames.Count) % miniGames.Count;
        string chosenGame = GetNextAvailableGame(finalIndex);

        // 🔐 Sauvegarde du jeu choisi
        PlayerPrefs.SetString("LastPlayedGame", chosenGame);
        PlayerPrefs.Save();

        // 💥 Animation et texte
        if (selectSound != null)
            audioSource.PlayOneShot(selectSound, 1f);

        yield return StartCoroutine(FlashWinner(miniGames[finalIndex].image));
        infoText.text = $"Mini-jeu sélectionné : <b>{chosenGame}</b>";
        Debug.Log($"🎯 Mini-jeu choisi : {chosenGame}");

        yield return new WaitForSeconds(1f);

        // 🎬 Si c’était le dernier jeu à jouer, on prépare le leaderboard
        if (remainingGames.Count == 1 && remainingGames.Contains(chosenGame))
        {
            // On le laisse encore se jouer, mais on vide la liste pour la prochaine fois
            remainingGames.Clear();
            SaveRemainingGames();
            Debug.Log("🕹 Dernier jeu du championnat lancé !");
        }

        yield return StartCoroutine(FadeOutMusicAndLoad(chosenGame));
    }

    private string GetNextAvailableGame(int startIndex)
    {
        if (remainingGames == null || remainingGames.Count == 0)
            return null;

        int safety = 0;
        int idx = startIndex;
        string chosen = null;

        while (safety < miniGames.Count)
        {
            string candidate = miniGames[idx].gameName;
            if (remainingGames.Contains(candidate))
            {
                chosen = candidate;
                break;
            }

            idx = (idx + 1) % miniGames.Count;
            safety++;
        }

        // fallback de sécurité
        if (chosen == null)
            chosen = remainingGames[0];

        return chosen;
    }

    private IEnumerator GoToLeaderboard()
    {
        Debug.Log("✅ Tous les mini-jeux ont été joués ! Passage au Leaderboard...");

        infoText.text = "Tous les mini-jeux ont été joués !";
        yield return new WaitForSeconds(2f);

        PlayerPrefs.DeleteKey("RemainingGames");
        PlayerPrefs.DeleteKey("LastPlayedGame");
        PlayerPrefs.Save();

        yield return StartCoroutine(FadeOutMusicAndLoad("Leaderboard"));
    }

    private void HighlightImage(int index)
    {
        for (int i = 0; i < miniGames.Count; i++)
        {
            var img = miniGames[i].image;
            if (img == null) continue;
            img.color = (i == index) ? highlightColor : normalColor;
            img.transform.localScale = (i == index) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private IEnumerator FlashWinner(Image winner)
    {
        if (winner == null) yield break;

        float time = 0f;
        float duration = 0.8f;
        Vector3 baseScale = winner.transform.localScale;

        while (time < duration)
        {
            float t = Mathf.PingPong(Time.time * 8f, 1f);
            winner.color = Color.Lerp(highlightColor, Color.white, t);
            winner.transform.localScale = Vector3.Lerp(baseScale, baseScale * 1.2f, t);
            time += Time.deltaTime;
            yield return null;
        }

        winner.color = Color.white;
        winner.transform.localScale = Vector3.one * 1.15f;

        if (flashOverlay != null)
            yield return StartCoroutine(FlashScreen());
    }

    private IEnumerator FlashScreen()
    {
        float fadeIn = 0.15f;
        float fadeOut = 0.4f;
        float t = 0f;

        while (t < fadeIn)
        {
            t += Time.deltaTime;
            flashOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, t / fadeIn));
            yield return null;
        }

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            flashOverlay.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, t / fadeOut));
            yield return null;
        }
    }

    private IEnumerator FadeOutMusicAndLoad(string sceneName)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            musicSource.Stop();
        }

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeOutAndLoad(sceneName));
        else
            SceneManager.LoadScene(sceneName);
    }

    private void SaveRemainingGames()
    {
        var data = new Wrapper { games = remainingGames };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("RemainingGames", json);
        PlayerPrefs.Save();
    }

    private void LoadRemainingGames()
    {
        if (PlayerPrefs.HasKey("RemainingGames"))
        {
            string json = PlayerPrefs.GetString("RemainingGames");
            var data = JsonUtility.FromJson<Wrapper>(json);
            remainingGames = new List<string>(data.games);
        }
        else
        {
            remainingGames = new List<string>();
            foreach (var slot in miniGames)
                remainingGames.Add(slot.gameName);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        selectionRoutine = null;
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}
