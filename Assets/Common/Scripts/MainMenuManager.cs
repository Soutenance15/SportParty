using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Boutons du menu principal")]
    public Button championnatButton;
    public Button duelButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Effet visuel")]
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

    [Header("Durée du fondu (en secondes)")]
    public float musicFadeDuration = 1.2f;

    private AudioSource audioSource;
    private static GameObject persistentMusicGO;
    private static AudioSource persistentMusicSource;

    private Button currentFocusedButton;
    private Coroutine pulseRoutine;
    private EventSystem eventSystem;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        eventSystem = EventSystem.current;

        LoadVolumeSettings();
        PlayMenuMusic();

        // --- Modes de jeu ---
        championnatButton.onClick.AddListener(() => OnSelectMode("PlayerSelectMenu", "Championship"));
        duelButton.onClick.AddListener(() => OnSelectMode("PlayerSelectMenu", "Duel")); // ✅ redirige aussi vers PlayerSelectMenu
        creditsButton.onClick.AddListener(() => OnSelectMode("Credits", ""));
        quitButton.onClick.AddListener(OnQuitGame);

        AddHoverEffect(championnatButton);
        AddHoverEffect(duelButton);
        AddHoverEffect(creditsButton);
        AddHoverEffect(quitButton);

        eventSystem.SetSelectedGameObject(championnatButton.gameObject);
        currentFocusedButton = championnatButton;
    }

    private void OnEnable()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.sendNavigationEvents = true;
            EventSystem.current.SetSelectedGameObject(championnatButton.gameObject);
        }
    }

    private void PlayMenuMusic()
    {
        if (menuMusic == null) return;

        if (persistentMusicGO == null)
        {
            persistentMusicGO = new GameObject("MenuMusicPlayer");
            persistentMusicSource = persistentMusicGO.AddComponent<AudioSource>();
            persistentMusicSource.clip = menuMusic;
            persistentMusicSource.loop = true;
            persistentMusicSource.spatialBlend = 0f;
            persistentMusicSource.volume = musicVolume;
            persistentMusicSource.Play();
            Object.DontDestroyOnLoad(persistentMusicGO);
        }
        else
        {
            persistentMusicSource.volume = musicVolume;
        }
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

    void Update()
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
        else
        {
            if (currentFocusedButton != null)
            {
                eventSystem.SetSelectedGameObject(currentFocusedButton.gameObject);
            }
            else
            {
                eventSystem.SetSelectedGameObject(championnatButton.gameObject);
                currentFocusedButton = championnatButton;
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

    // ✅ Gère le mode avant chargement
    private void OnSelectMode(string sceneName, string mode)
    {
        if (!string.IsNullOrEmpty(mode))
            GameDataManager.SetGameMode(mode);

        Debug.Log($"Chargement de la scène : {sceneName} (mode {mode})");

        if (selectSound != null)
        {
            GameObject soundGO = new GameObject("TempSelectSound");
            AudioSource tempAudio = soundGO.AddComponent<AudioSource>();
            tempAudio.clip = selectSound;
            tempAudio.volume = sfxVolume;
            tempAudio.spatialBlend = 0f;
            tempAudio.Play();
            Object.DontDestroyOnLoad(soundGO);
            Object.Destroy(soundGO, selectSound.length);
        }

        StartCoroutine(FadeOutMusicAndLoad(sceneName));
    }

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
        {
            yield return fader.FadeOutAndLoad(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void OnQuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}
