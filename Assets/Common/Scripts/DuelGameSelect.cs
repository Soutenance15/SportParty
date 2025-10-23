using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class DuelGameSelect : MonoBehaviour
{
    [Header("Boutons du menu")]
    public Button pingPongButton;
    public Button kartButton;
    public Button footButton;
    public Button paraglideButton;

    [Header("Effet visuel")]
    public float pulseScale = 1.1f;
    public float pulseSpeed = 2f;
    public Color highlightColor = new Color(1f, 0.8f, 0.2f); // Doré clair

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

    private Button currentHoveredButton;
    private Coroutine pulseRoutine;
    private EventSystem eventSystem;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        eventSystem = EventSystem.current;

        // 🔊 Charge les préférences de volume
        LoadVolumeSettings();

        // 🎵 Joue la musique de fond du menu Duel
        PlayMenuMusic();

        // Liens boutons → scènes
        pingPongButton.onClick.AddListener(() => OnSelectMiniGame("PingPong"));
        kartButton.onClick.AddListener(() => OnSelectMiniGame("Karting"));
        footButton.onClick.AddListener(() => OnSelectMiniGame("Foot"));
        paraglideButton.onClick.AddListener(() => OnSelectMiniGame("paraglide"));

        // Ajoute les effets de survol souris
        AddHoverEffect(pingPongButton);
        AddHoverEffect(kartButton);
        AddHoverEffect(footButton);
        AddHoverEffect(paraglideButton);

        // Sélection par défaut
        eventSystem.SetSelectedGameObject(pingPongButton.gameObject);
        currentHoveredButton = pingPongButton;
    }

    private void OnEnable()
    {
        // ✅ Empêche la perte du focus manette après un clic souris
        if (EventSystem.current != null)
        {
            EventSystem.current.sendNavigationEvents = true;
            EventSystem.current.SetSelectedGameObject(pingPongButton.gameObject);
        }
    }

    private void PlayMenuMusic()
    {
        if (menuMusic == null) return;

        if (persistentMusicGO == null)
        {
            persistentMusicGO = new GameObject("DuelMenuMusicPlayer");
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

    // 🔧 Volume
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
        // 🕹️ Navigation manette / clavier
        if (eventSystem.currentSelectedGameObject != null)
        {
            Button selected = eventSystem.currentSelectedGameObject.GetComponent<Button>();
            if (selected != null && selected != currentHoveredButton)
            {
                if (currentHoveredButton != null)
                    OnButtonUnfocus(currentHoveredButton);

                OnButtonFocus(selected);
            }
        }
        else
        {
            // ✅ Si la souris casse la sélection, on la restaure automatiquement
            if (currentHoveredButton != null)
            {
                eventSystem.SetSelectedGameObject(currentHoveredButton.gameObject);
            }
            else
            {
                eventSystem.SetSelectedGameObject(pingPongButton.gameObject);
                currentHoveredButton = pingPongButton;
            }
        }
    }

    private void OnButtonFocus(Button btn)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        currentHoveredButton = btn;
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

    // ✅ Sélection du mini-jeu
    private void OnSelectMiniGame(string sceneName)
    {
        Debug.Log($"Chargement de la scène : {sceneName}");

        if (selectSound != null)
        {
            GameObject tempSoundGO = new GameObject("TempSelectSound");
            AudioSource tempAudio = tempSoundGO.AddComponent<AudioSource>();
            tempAudio.clip = selectSound;
            tempAudio.volume = sfxVolume;
            tempAudio.spatialBlend = 0f;
            tempAudio.Play();
            DontDestroyOnLoad(tempSoundGO);
            Destroy(tempSoundGO, selectSound.length);
        }

        StartCoroutine(FadeOutMusicAndLoad(sceneName));
    }

    // 🔙 Retour menu principal
    public void OnReturnToMenu()
    {
        Debug.Log("Retour au menu principal...");

        if (selectSound != null)
        {
            GameObject tempSoundGO = new GameObject("TempSelectSound");
            AudioSource tempAudio = tempSoundGO.AddComponent<AudioSource>();
            tempAudio.clip = selectSound;
            tempAudio.volume = sfxVolume;
            tempAudio.spatialBlend = 0f;
            tempAudio.Play();
            DontDestroyOnLoad(tempSoundGO);
            Destroy(tempSoundGO, selectSound.length);
        }

        StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
    }

    // 🎧 Fade musical progressif avant le changement de scène
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
}
