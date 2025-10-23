using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSelectMenuManager : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_InputField inputPlayer1;
    public TMP_InputField inputPlayer2;
    public Button startButton;
    public Button returnButton;
    public TMP_Text titleText; // optionnel : titre dynamique

    [Header("Audio - Musique de fond")]
    public AudioClip menuMusic;
    private AudioSource musicSource;
    private float musicVolume = 0.5f;

    [Header("Audio - Feedbacks")]
    public AudioClip selectSound;
    public AudioClip validateSound;
    private AudioSource sfxSource;

    [Header("Nom de la scène du sélecteur de mini-jeux")]
    public string miniGameSelectorScene = "MiniGameSelector";

    private EventSystem eventSystem;
    private Selectable currentSelected;
    private Coroutine fadeRoutine;

    private void Start()
    {
        eventSystem = EventSystem.current;

        // --- 🎵 Musique du menu ---
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.Play();
        }

        // --- 🔊 SFX ---
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.9f);

        // --- Préremplit les noms ---
        inputPlayer1.text = GameDataManager.Player1;
        inputPlayer2.text = GameDataManager.Player2;

        // --- Feedback sonore sur sélection ---
        inputPlayer1.onSelect.AddListener(delegate { PlaySelectSound(); });
        inputPlayer2.onSelect.AddListener(delegate { PlaySelectSound(); });

        // --- Focus par défaut ---
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(inputPlayer1.gameObject);
            currentSelected = inputPlayer1;
        }

        // --- Boutons ---
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);

        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnToMainMenu);

        // --- Affiche le mode ---
        string mode = GameDataManager.GetGameMode();
        if (titleText != null)
            titleText.text = mode == "Duel" ? "DUEL" : "CHAMPIONNAT";
    }

    private void Update()
    {
        if (eventSystem == null) return;

        if (eventSystem.currentSelectedGameObject == null)
        {
            if (currentSelected != null)
                eventSystem.SetSelectedGameObject(currentSelected.gameObject);
            else
            {
                eventSystem.SetSelectedGameObject(inputPlayer1.gameObject);
                currentSelected = inputPlayer1;
            }
        }
        else
        {
            var newSelectable = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
            if (newSelectable != null && newSelectable != currentSelected)
            {
                currentSelected = newSelectable;
                PlaySelectSound();
            }
        }
    }

    private void PlaySelectSound()
    {
        if (selectSound != null && sfxSource != null)
            sfxSource.PlayOneShot(selectSound, 0.8f);
    }

    private void PlayValidateSound()
    {
        if (validateSound != null && sfxSource != null)
            sfxSource.PlayOneShot(validateSound, 1f);
    }

    // --- 🚀 Lancement du jeu (Duel ou Championnat) ---
    public void OnStartGame()
    {
        string p1 = inputPlayer1.text.Trim();
        string p2 = inputPlayer2.text.Trim();

        if (string.IsNullOrEmpty(p1)) p1 = "Joueur 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Joueur 2";

        string mode = GameDataManager.GetGameMode();
        string nextScene = (mode == "Duel") ? "DuelGameSelect" : miniGameSelectorScene;

        // 💾 Sauvegarde des noms
        GameDataManager.SavePlayers(p1, p2);

        // 🧹 Reset complet uniquement pour le championnat
        if (mode == "Championship")
        {
            GameDataManager.ResetAll();
            PlayerPrefs.DeleteKey("RemainingGames");
            PlayerPrefs.DeleteKey("LastPlayedGame");
            PlayerPrefs.Save();
        }

        Debug.Log($"🏁 Lancement d'une partie ({mode}) : {p1} vs {p2}");

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutMusicAndLoad(nextScene));
    }

    public void OnReturnToMainMenu()
    {
        PlayValidateSound();

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
    }

    private IEnumerator FadeOutMusicAndLoad(string sceneName)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float duration = 1.2f;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (musicSource != null)
                    musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            if (musicSource != null)
                musicSource.Stop();
        }

        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneName);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        fadeRoutine = null;

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }
}
