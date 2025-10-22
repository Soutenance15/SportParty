using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MiniGameSelector : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text infoText;
    public List<Image> miniGameImages; // Assigné dans l’inspecteur
    public Color highlightColor = new Color(1f, 0.85f, 0.2f); // Doré lumineux
    public Color normalColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Audio")]
    public AudioClip selectSound;      // 🔊 Son final (jeu choisi)
    public AudioClip whooshSound;      // 🔊 Petit son à chaque survol
    public AudioClip backgroundMusic;  // 🎵 Musique du sélecteur
    public float musicVolume = 0.8f;
    public float fadeOutDuration = 1.5f;
    private AudioSource audioSource;
    private AudioSource musicSource;

    [Header("Effet visuel")]
    public Image flashOverlay;         // ✅ Une image blanche plein écran avec alpha 0

    private static List<string> remainingGames = new List<string>()
    {
        "PingPong",
        "Karting",
        "Parapente",
        "Foot"
    };

    private ScreenFader screenFader;

    private void Start()
    {
        screenFader = UnityEngine.Object.FindFirstObjectByType<ScreenFader>();

        // 🎧 Gestion des sources audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }

        // 🔆 Réinitialisation du flash
        if (flashOverlay != null)
            flashOverlay.color = new Color(1f, 1f, 1f, 0f);

        StartCoroutine(SelectMiniGameWithAnimation());
    }

    private IEnumerator SelectMiniGameWithAnimation()
    {
        if (remainingGames.Count == 0)
        {
            infoText.text = "Tous les mini-jeux ont été joués !";
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FadeOutMusicAndLoad("Leaderboard"));
            yield break;
        }

        infoText.text = "Sélection aléatoire du mini-jeu...";
        yield return new WaitForSeconds(0.8f);

        int highlightIndex = 0;
        float delay = 0.12f;
        int rounds = Random.Range(12, 18);

        // 🔁 Animation de survol
        for (int i = 0; i < rounds; i++)
        {
            HighlightImage(highlightIndex);

            if (whooshSound != null)
                audioSource.PlayOneShot(whooshSound, 0.7f);

            highlightIndex = (highlightIndex + 1) % miniGameImages.Count;
            yield return new WaitForSeconds(delay);
            delay *= 1.09f;
        }

        // 🏁 Jeu choisi
        int finalIndex = (highlightIndex - 1 + miniGameImages.Count) % miniGameImages.Count;
        string chosenGame = GetAvailableGame(finalIndex);
        if (chosenGame == null)
        {
            chosenGame = remainingGames[Random.Range(0, remainingGames.Count)];
        }

        remainingGames.Remove(chosenGame);

        // 💥 Effet flash et son final
        if (selectSound != null)
            audioSource.PlayOneShot(selectSound, 1f);

        yield return StartCoroutine(FlashWinner(miniGameImages[finalIndex]));

        infoText.text = $"Mini-jeu sélectionné : <b>{chosenGame}</b>";
        Debug.Log($"🎯 Mini-jeu choisi : {chosenGame}");

        yield return new WaitForSeconds(1f);

        // 🎧 Fade musical progressif avant chargement
        yield return StartCoroutine(FadeOutMusicAndLoad(chosenGame));
    }

    private void HighlightImage(int index)
    {
        for (int i = 0; i < miniGameImages.Count; i++)
        {
            miniGameImages[i].color = (i == index) ? highlightColor : normalColor;
            miniGameImages[i].transform.localScale = (i == index) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private IEnumerator FlashWinner(Image winner)
    {
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

    // 🎧 Fade musical avant le chargement de la scène
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

        // ⏳ Transition visuelle ensuite
        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeOutAndLoad(sceneName));
        else
            SceneManager.LoadScene(sceneName);
    }

    // ✅ Vérifie si le jeu à cet index est encore dispo
    private string GetAvailableGame(int index)
    {
        if (index < 0 || index >= 4) return null;

        string[] allGames = { "PingPong", "Karting", "Parapente", "Foot" };
        string game = allGames[index];

        if (remainingGames.Contains(game))
            return game;
        else
            return null;
    }
}
