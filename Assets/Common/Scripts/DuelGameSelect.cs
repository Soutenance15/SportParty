using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class DuelGameSelect : MonoBehaviour
{
    [Header("Boutons du menu Duel")]
    public Button pingPongButton;
    public Button kartButton;
    public Button footButton;
    public Button paraglideButton;
    public Button returnButton;

    [Header("Effet visuel")]
    public float pulseScale = 1.1f;
    public float pulseSpeed = 2f;
    public Color highlightColor = new Color(1f, 0.8f, 0.2f);

    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip selectSound;
    public AudioClip menuMusic;

    [Header("Réglages audio (0 à 1)")]
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

    // 🎮 / 🖱️ gestion hybride
    private bool usingMouse = false;
    private float mouseInactiveTimer = 0f;
    private const float mouseTimeout = 1.5f;
    private bool inputLocked = false;

    void Start()
    {
        // ⚙️ Mode Duel
        GameDataManager.SetGameMode("Duel");

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        eventSystem = EventSystem.current;
        LoadVolumeSettings();
        PlayMenuMusic();

        // 🔗 Liens boutons → mini-jeux
        pingPongButton.onClick.AddListener(() => OnSelectMiniGame("PingPong"));
        kartButton.onClick.AddListener(() => OnSelectMiniGame("Karting"));
        footButton.onClick.AddListener(() => OnSelectMiniGame("Foot"));
        paraglideButton.onClick.AddListener(() => OnSelectMiniGame("Parapente"));
        if (returnButton != null)
            returnButton.onClick.AddListener(OnReturnToMenu);

        // 💡 Ajout des effets de hover
        AddHoverEffect(pingPongButton);
        AddHoverEffect(kartButton);
        AddHoverEffect(footButton);
        AddHoverEffect(paraglideButton);
        if (returnButton != null) AddHoverEffect(returnButton);

        // 🎯 Focus initial
        FocusButton(pingPongButton);
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
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

    void Update()
    {
        if (eventSystem == null || inputLocked) return;

        // 🖱️ Détection souris active
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
        {
            if (!usingMouse)
            {
                usingMouse = true;
                eventSystem.SetSelectedGameObject(null);
            }
            mouseInactiveTimer = 0f;
        }
        else if (usingMouse)
        {
            mouseInactiveTimer += Time.unscaledDeltaTime;
            if (mouseInactiveTimer > mouseTimeout)
                usingMouse = false;
        }

        // 🖱️ Gestion clics souris
        if (usingMouse && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var button = result.gameObject.GetComponent<Button>();
                if (button != null && button.interactable)
                {
                    button.onClick.Invoke();
                    PlaySelectSound();
                    return;
                }
            }
        }

        // 🎮 Navigation manette
        if (!usingMouse)
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                if (currentFocusedButton != null)
                    eventSystem.SetSelectedGameObject(currentFocusedButton.gameObject);
                else
                    eventSystem.SetSelectedGameObject(pingPongButton.gameObject);
            }

            Button selected = eventSystem.currentSelectedGameObject?.GetComponent<Button>();
            if (selected != null && selected != currentFocusedButton)
            {
                if (currentFocusedButton != null)
                    UnfocusButton(currentFocusedButton);

                FocusButton(selected);
            }
        }
    }

    private void AddHoverEffect(Button btn)
    {
        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) =>
        {
            if (!usingMouse) return;
            FocusButton(btn);
        });
        trigger.triggers.Add(entryEnter);

        var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) =>
        {
            if (!usingMouse) return;
            UnfocusButton(btn);
        });
        trigger.triggers.Add(entryExit);
    }

    private void FocusButton(Button btn)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        currentFocusedButton = btn;
        pulseRoutine = StartCoroutine(PulseEffect(btn.transform));

        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null) txt.color = highlightColor;

        PlayHoverSound();
    }

    private void UnfocusButton(Button btn)
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
            float scale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * (pulseScale - 1f);
            target.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound, sfxVolume);
    }

    private void PlaySelectSound()
    {
        if (selectSound != null && audioSource != null)
            audioSource.PlayOneShot(selectSound, sfxVolume);
    }

    private void OnSelectMiniGame(string sceneName)
    {
        if (inputLocked) return;
        inputLocked = true;

        Debug.Log($"Chargement du mini-jeu : {sceneName}");
        PlaySelectSound();
        StartCoroutine(FadeOutMusicAndLoad(sceneName));
    }

    public void OnReturnToMenu()
    {
        if (inputLocked) return;
        inputLocked = true;

        Debug.Log("↩️ Retour au menu principal...");
        PlaySelectSound();
        StartCoroutine(FadeOutMusicAndLoad("MainMenu"));
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
            yield return fader.FadeOutAndLoad(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }
}
