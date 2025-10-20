using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PlayerSelectMenuManager : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_InputField inputPlayer1;
    public TMP_InputField inputPlayer2;
    public Button startButton;
    public Button returnButton;

    [Header("Nom de la scène de sélection aléatoire")]
    public string miniGameSelectorScene = "MiniGameSelector";

    [Header("Effets visuels")]
    public float pulseScale = 1.1f;
    public float pulseSpeed = 2f;
    public Color highlightColor = new Color(1f, 0.8f, 0.2f);

    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip menuMusic;

    [Header("Réglages de volume (0 à 1)")]
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 0.9f;

    [Header("Durée du fondu musical")]
    public float musicFadeDuration = 1.2f;

    private AudioSource audioSource;
    private static GameObject persistentMusicGO;
    private static AudioSource persistentMusicSource;

    private Button currentFocusedButton;
    private Coroutine pulseRoutine;
    private EventSystem eventSystem;

    private void Start()
    {
        eventSystem = EventSystem.current;
        audioSource = gameObject.AddComponent<AudioSource>();

        // 🔊 Charge les réglages de volume sauvegardés
        LoadVolumeSettings();

        // 🎵 Lance la musique du menu
        PlayMenuMusic();

        // Pré-remplit les champs
        inputPlayer1.text = GameDataManager.Player1;
        inputPlayer2.text = GameDataManager.Player2;

        // Ajout des listeners
        startButton.onClick.AddListener(OnStartGame);
        returnButton.onClick.AddListener(OnReturnToMainMenu);

        // Ajoute les effets visuels
        AddHoverEffect(startButton);
        AddHoverEffect(returnButton);

        // Focus par défaut manette
        eventSystem.SetSelectedGameObject(startButton.gameObject);
    }

    // 🎵 Joue la musique de fond
    private void PlayMenuMusic()
    {
        if (menuMusic == null) return;

        if (persistentMusicGO == null)
        {
            persistentMusicGO = new GameObject("PlayerSelectMusicPlayer");
            persistentMusicSource = persistentMusicGO.AddComponent<AudioSource>();
            persistentMusicSource.clip = menuMusic;
            persistentMusicSource.loop = true;
            persistentMusicSource.volume = musicVolume;
            persistentMusicSource.spatialBlend = 0f;
            persistentMusicSource.Play();
            DontDestroyOnLoad(persistentMusicGO);
        }
        else
        {
            persistentMusicSource.volume = musicVolume;
        }
    }

    // 🔧 Réglages de volume
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();

        if (persistentMusicSource != null)
            persistentMusicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
    }

    private void AddHoverEffect(Button btn)
    {
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => OnButtonFocus(btn));
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => OnButtonUnfocus(btn));
        trigger.triggers.Add(entryExit);
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            Button selected = eventSystem.currentSelectedGameObject.GetComponent<Button>();
            if (selected != null && selected != currentFocusedButton)
            {
                if (currentFocusedButton != null)
                    OnButtonUnfocus(currentFocusedButton);

                OnButtonFocus(selected);
            }
        }
    }

    private void OnButtonFocus(Button btn)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        currentFocusedButton = btn;
        pulseRoutine = StartCoroutine(PulseEffect(btn.transform));

        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.color = highlightColor;

        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound, sfxVolume);
    }

    private void OnButtonUnfocus(Button btn)
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        btn.transform.localScale = Vector3.one;
        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.color = Color.white;
    }

    private IEnumerator PulseEffect(Transform target)
    {
        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
            target.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    // 🚀 Démarrage du jeu avec fade musical
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

        // 🔊 Son de validation
        if (selectSound != null)
        {
            GameObject tempAudioGO = new GameObject("TempSelectSound");
            AudioSource tempSource = tempAudioGO.AddComponent<AudioSource>();
            tempSource.clip = selectSound;
            tempSource.volume = sfxVolume;
            tempSource.spatialBlend = 0f;
            tempSource.Play();
            DontDestroyOnLoad(tempAudioGO);
            Destroy(tempAudioGO, selectSound.length);
        }

        StartCoroutine(FadeOutMusicAndLoad(miniGameSelectorScene));
    }

    // 🔙 Retour au menu principal avec fade musical
    public void OnReturnToMainMenu()
    {
        Debug.Log("Retour au menu principal...");

        if (selectSound != null)
        {
            GameObject tempAudioGO = new GameObject("TempSelectSound");
            AudioSource tempSource = tempAudioGO.AddComponent<AudioSource>();
            tempSource.clip = selectSound;
            tempSource.volume = sfxVolume;
            tempSource.spatialBlend = 0f;
            tempSource.Play();
            DontDestroyOnLoad(tempAudioGO);
            Destroy(tempAudioGO, selectSound.length);
        }

        StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
    }

    // 🎧 Fade-out musical avant transition
    private IEnumerator FadeOutMusicAndLoad(string sceneName)
    {
        if (persistentMusicSource != null)
        {
            float startVolume = persistentMusicSource.volume;
            float elapsed = 0f;

            while (elapsed < musicFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                persistentMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / musicFadeDuration);
                yield return null;
            }

            persistentMusicSource.Stop();
            Destroy(persistentMusicGO);
            persistentMusicGO = null;
            persistentMusicSource = null;
        }

        yield return FadeAndLoad(sceneName);
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader != null)
            yield return fader.FadeOutAndLoad(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
