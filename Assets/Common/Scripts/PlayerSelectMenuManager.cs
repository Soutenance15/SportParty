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

    // 🧩 Clavier virtuel Unity
    private TouchScreenKeyboard keyboard;
    private TMP_InputField currentInputField;

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

        // --- Sons & clavier sur focus ---
        inputPlayer1.onSelect.AddListener(delegate { OnFieldSelected(inputPlayer1); });
        inputPlayer2.onSelect.AddListener(delegate { OnFieldSelected(inputPlayer2); });

        // --- Focus par défaut ---
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(inputPlayer1.gameObject);
            currentSelected = inputPlayer1;
        }

        // --- Boutons ---
        if (startButton != null)
            startButton.onClick.AddListener(() => PlayValidateSound());

        if (returnButton != null)
            returnButton.onClick.AddListener(() => PlayValidateSound());
    }

    private void Update()
    {
        if (eventSystem == null) return;

        // ✅ Navigation manette/souris
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

    // --- 🔊 SFX ---
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

    // --- 🧩 Gestion de la sélection d’un champ ---
    private void OnFieldSelected(TMP_InputField targetField)
    {
        PlaySelectSound();

        // ✅ Ouvre le clavier Unity ou Windows
        OpenVirtualKeyboard(targetField);
    }

    private void OpenVirtualKeyboard(TMP_InputField targetField)
    {
        currentInputField = targetField;

#if UNITY_STANDALONE_WIN
        // 💻 Sur PC : ouvre le clavier virtuel Windows
        try
        {
            System.Diagnostics.Process.Start("osk.exe");
        }
        catch
        {
            Debug.LogWarning("⚠️ Impossible d’ouvrir le clavier virtuel Windows (osk.exe)");
        }
#else
        // 🎮 Sur manette / mobile / console : ouvre le clavier Unity natif
        keyboard = TouchScreenKeyboard.Open(
            targetField.text,
            TouchScreenKeyboardType.Default,
            false, false, false, false,
            "Entrez le nom du joueur"
        );

        StartCoroutine(WaitForKeyboardInput(targetField));
#endif
    }

    private IEnumerator WaitForKeyboardInput(TMP_InputField targetField)
    {
        while (keyboard != null && !keyboard.done && !keyboard.wasCanceled)
            yield return null;

        if (keyboard != null && !keyboard.wasCanceled)
            targetField.text = keyboard.text;

        keyboard = null;
    }

    // --- 🚀 Lancement du championnat ---
    public void OnStartGame()
    {
        string p1 = inputPlayer1.text.Trim();
        string p2 = inputPlayer2.text.Trim();

        if (string.IsNullOrEmpty(p1)) p1 = "Joueur 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Joueur 2";

        // 🧹 Réinitialise les données
        GameDataManager.ResetAll();
        PlayerPrefs.DeleteKey("RemainingGames");
        PlayerPrefs.DeleteKey("LastPlayedGame");
        PlayerPrefs.Save();

        // 💾 Sauvegarde les noms
        GameDataManager.SavePlayers(p1, p2);
        PlayerPrefs.Save();

        Debug.Log($"🏁 Nouvelle partie lancée : {p1} vs {p2}");

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutMusicAndLoad(miniGameSelectorScene));
    }

    // --- 🔙 Retour menu principal ---
    public void OnReturnToMainMenu()
    {
        Debug.Log("↩️ Retour au menu principal...");
        PlayValidateSound();

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
    }

    // --- 🎧 Transition musicale ---
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
