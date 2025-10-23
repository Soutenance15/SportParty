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
    public TMP_Text titleText;

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

    private bool usingMouse = false;
    private float mouseInactiveTimer = 0f;
    private const float mouseTimeout = 1.5f;

    private void Start()
    {
        eventSystem = EventSystem.current;

        // 🎵 Musique
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

        // 🔊 SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.9f);

        // 🧾 Chargement noms sauvegardés
        inputPlayer1.text = GameDataManager.Player1;
        inputPlayer2.text = GameDataManager.Player2;

        inputPlayer1.onSelect.AddListener(delegate { PlaySelectSound(); });
        inputPlayer2.onSelect.AddListener(delegate { PlaySelectSound(); });

        // 🎮 Boutons
        startButton.onClick.AddListener(OnStartGame);
        returnButton.onClick.AddListener(OnReturnToMainMenu);

        // 🏁 Titre
        string mode = GameDataManager.GetGameMode();
        if (titleText != null)
            titleText.text = mode == "Duel" ? "DUEL" : "CHAMPIONNAT";

        // 🎯 Focus initial
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(inputPlayer1.gameObject);
            currentSelected = inputPlayer1;
        }
    }

    private void Update()
    {
        if (eventSystem == null) return;

        // 🖱️ Détection activité souris
        if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
        {
            usingMouse = true;
            mouseInactiveTimer = 0f;
        }
        else if (usingMouse)
        {
            mouseInactiveTimer += Time.unscaledDeltaTime;
            if (mouseInactiveTimer > mouseTimeout)
                usingMouse = false;
        }

        // 🖱️ Gestion du clic souris
        if (usingMouse && Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            eventSystem.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke(); // ✅ Simule un vrai clic Unity
                    PlayValidateSound();
                    return;
                }

                var input = result.gameObject.GetComponent<TMP_InputField>();
                if (input != null)
                {
                    eventSystem.SetSelectedGameObject(input.gameObject);
                    input.Select();
                    input.ActivateInputField();
                    PlaySelectSound();
                    return;
                }
            }
        }

        // 🎮 Navigation manette / clavier
        if (!usingMouse)
        {
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

    public void OnStartGame()
    {
        string p1 = inputPlayer1.text.Trim();
        string p2 = inputPlayer2.text.Trim();

        if (string.IsNullOrEmpty(p1)) p1 = "Joueur 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Joueur 2";

        string mode = GameDataManager.GetGameMode();
        string nextScene = (mode == "Duel") ? "DuelGameSelect" : miniGameSelectorScene;

        GameDataManager.SavePlayers(p1, p2);

        if (mode == "Championship")
        {
            GameDataManager.ResetAll();
            PlayerPrefs.DeleteKey("RemainingGames");
            PlayerPrefs.DeleteKey("LastPlayedGame");
            PlayerPrefs.Save();
            GameDataManager.SavePlayers(p1, p2);
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
        float duration = 1f;
        float startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
