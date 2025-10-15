using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton optionnel
    public AudioSource musicSource;
    public AudioClip gameMusic;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool loop = true;

    private void Awake()
    {
        // ✅ Empêche la duplication entre les scènes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // garde la musique d’une scène à l’autre
    }

    private void Start()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (gameMusic != null)
        {
            musicSource.clip = gameMusic;
            musicSource.loop = loop;
            musicSource.volume = volume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("🎵 Aucun clip musical assigné au MusicManager !");
        }
    }

    // 🕹️ Méthodes de contrôle
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.UnPause();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}
