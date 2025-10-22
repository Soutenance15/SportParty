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
    public AudioClip selectSound;   // 🔊 pour clic ou focus champ
    public AudioClip validateSound; // 🔊 pour validation du lancement
    private AudioSource sfxSource;  // Source séparée pour éviter les conflits

    [Header("Nom de la scène de sélection aléatoire")]
    public string miniGameSelectorScene = "MiniGameSelector";

    private EventSystem eventSystem;
    private Selectable currentSelected;

    private void Start()
    {
        eventSystem = EventSystem.current;

        // --- 🎵 Prépare la musique ---
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

        // --- 🔊 Source SFX ---
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = musicVolume;

        // --- Pré-remplit les pseudos ---
        inputPlayer1.text = GameDataManager.Player1;
        inputPlayer2.text = GameDataManager.Player2;

        // --- Connecte les sons de saisie ---
        inputPlayer1.onSelect.AddListener(delegate { PlaySelectSound(); });
        inputPlayer2.onSelect.AddListener(delegate { PlaySelectSound(); });

        // --- Focus par défaut ---
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(inputPlayer1.gameObject);
            currentSelected = inputPlayer1;
        }

        // --- Sons sur les boutons ---
        if (startButton != null)
            startButton.onClick.AddListener(() => PlayValidateSound());

        if (returnButton != null)
            returnButton.onClick.AddListener(() => PlayValidateSound());
    }

    private void Update()
    {
        // ✅ Maintient la compatibilité manette ↔ souris
        if (eventSystem.currentSelectedGameObject == null)
        {
            // Si le focus est perdu après un clic, on restaure le dernier champ ou bouton
            if (currentSelected != null)
            {
                eventSystem.SetSelectedGameObject(currentSelected.gameObject);
            }
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

    // --- 🔊 Lecture des SFX ---
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

    // --- 🚀 Lancement de la partie ---
    public void OnStartGame()
    {
        string p1 = inputPlayer1.text.Trim();
        string p2 = inputPlayer2.text.Trim();

        if (string.IsNullOrEmpty(p1)) p1 = "Joueur 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Joueur 2";

        GameDataManager.SavePlayers(p1, p2);

        PlayerPrefs.DeleteKey(p1 + GameDataManager.CHAMP_POINTS_SUFFIX);
        PlayerPrefs.DeleteKey(p2 + GameDataManager.CHAMP_POINTS_SUFFIX);
        PlayerPrefs.Save();

        Debug.Log($"Nouvelle partie lancée : {p1} vs {p2}");

        PlayValidateSound();
        StartCoroutine(FadeOutMusicAndLoad(miniGameSelectorScene));
    }

    // --- 🔙 Retour au menu principal ---
    public void OnReturnToMainMenu()
    {
        Debug.Log("Retour au menu principal...");
        PlayValidateSound();
        StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
    }

    // --- 🎧 Fade musical et chargement ---
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
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
                yield return null;
            }

            musicSource.Stop();
        }

        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(sceneName);
    }
}
