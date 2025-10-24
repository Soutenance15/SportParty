using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [Header("Références UI")]
    public TMP_Text leaderboardText;
    public Button replayButton;
    public Button returnButton;

    [Header("Audio")]
    public AudioClip leaderboardMusic;
    public AudioClip hoverSound;
    public AudioClip selectSound;
    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float musicVolume = 0.5f;
    private float sfxVolume = 0.9f;

    private EventSystem eventSystem;
    private Button currentFocusedButton;
    private Coroutine pulseRoutine;

    [Header("Effet visuel")]
    public float pulseScale = 1.08f;
    public float pulseSpeed = 2f;
    public Color highlightColor = new Color(1f, 0.85f, 0.2f);

    [Header("Animation vainqueur")]
    public Color winnerColor = new Color(1f, 0.8f, 0.2f);
    public float winnerPulseSpeed = 2f;
    public float winnerPulseScale = 1.1f;
    public float winnerAnimDuration = 3f;

    // 🖱️ / 🎮 gestion hybride
    private bool usingMouse = false;
    private float mouseInactiveTimer = 0f;
    private const float mouseTimeout = 1.5f;

    void Start()
    {
        eventSystem = EventSystem.current;

        // 🎵 Musique
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.9f);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;

        if (leaderboardMusic != null)
        {
            musicSource.clip = leaderboardMusic;
            musicSource.Play();
        }

        // 🔊 SFX séparé
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;

        // 🏆 Affiche le classement
        DisplayLeaderboard();

        // 🔘 Boutons
        if (replayButton != null)
        {
            replayButton.onClick.AddListener(OnReplay);
            AddHoverEffect(replayButton);
        }

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(() => OnButtonSelect("MainMenu"));
            AddHoverEffect(returnButton);
        }

        StartCoroutine(SetupInitialFocus());
    }

    private IEnumerator SetupInitialFocus()
    {
        yield return null;
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (replayButton != null)
        {
            eventSystem.SetSelectedGameObject(replayButton.gameObject);
            currentFocusedButton = replayButton;
        }
    }

    void Update()
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

        // 🖱️ Gestion clic souris
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
                if (button != null)
                {
                    button.onClick.Invoke();
                    PlaySelectSound();
                    OnButtonFocus(button);
                    return;
                }
            }
        }

        // 🎮 Navigation manette
        if (!usingMouse)
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
                    eventSystem.SetSelectedGameObject(currentFocusedButton.gameObject);
                else if (replayButton != null)
                {
                    eventSystem.SetSelectedGameObject(replayButton.gameObject);
                    currentFocusedButton = replayButton;
                }
            }
        }
    }

    private void DisplayLeaderboard()
    {
        if (leaderboardText == null)
            return;

        string p1 = GameDataManager.Player1;
        string p2 = GameDataManager.Player2;

        int champP1 = GameDataManager.GetChampPoints(p1);
        int champP2 = GameDataManager.GetChampPoints(p2);

        leaderboardText.text = "CLASSEMENT FINAL\n\n";
        leaderboardText.text += $"{p1} : {champP1} pts\n";
        leaderboardText.text += $"{p2} : {champP2} pts\n\n";

        if (champP1 > champP2)
        {
            leaderboardText.text += $"Vainqueur : <b><color=#FFD24A>{p1}</color></b>";
            StartCoroutine(AnimateWinnerText(p1));
        }
        else if (champP2 > champP1)
        {
            leaderboardText.text += $"Vainqueur : <b><color=#FFD24A>{p2}</color></b>";
            StartCoroutine(AnimateWinnerText(p2));
        }
        else
        {
            leaderboardText.text += "Égalité parfaite !";
            StartCoroutine(AnimateWinnerText("Égalité"));
        }
    }

    // ✨ Animation du vainqueur
    private IEnumerator AnimateWinnerText(string winnerName)
    {
        float timer = 0f;
        TMP_Text tmp = leaderboardText;

        Vector3 baseScale = tmp.transform.localScale;
        Color baseColor = Color.white;
        Color pulseColor = (winnerName == "Égalité") ? Color.white : winnerColor;

        while (timer < winnerAnimDuration)
        {
            float t = Mathf.Sin(Time.time * winnerPulseSpeed) * 0.5f + 0.5f;
            tmp.color = Color.Lerp(baseColor, pulseColor, t);
            tmp.transform.localScale = baseScale * Mathf.Lerp(1f, winnerPulseScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        tmp.color = baseColor;
        tmp.transform.localScale = baseScale;
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

    private void OnButtonFocus(Button btn)
    {
        if (btn == null) return;

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        currentFocusedButton = btn;
        pulseRoutine = StartCoroutine(PulseEffect(btn.transform));

        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.color = highlightColor;

        PlayHoverSound();
    }

    private void OnButtonUnfocus(Button btn)
    {
        if (btn == null) return;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        btn.transform.localScale = Vector3.one;
        TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.color = Color.white;
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

    private void PlayHoverSound()
    {
        if (hoverSound != null && sfxSource != null)
            sfxSource.PlayOneShot(hoverSound, 0.8f);
    }

    private void PlaySelectSound()
    {
        if (selectSound != null && sfxSource != null)
            sfxSource.PlayOneShot(selectSound, 1f);
    }

    private void OnButtonSelect(string sceneName)
    {
        PlaySelectSound();
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
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

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(sceneName);
    }

    public void OnReplay()
    {
        Debug.Log("🏁 Nouveau championnat lancé !");
        PlaySelectSound();

        GameDataManager.ResetAll();
        PlayerPrefs.DeleteKey("RemainingGames");
        PlayerPrefs.Save();

        StartCoroutine(FadeOutAndLoad("PlayerSelectMenu"));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        pulseRoutine = null;
    }
}
