using UnityEngine;
using UnityEngine.UI;

public class FootSoundManager : MonoBehaviour
{
    private static FootSoundManager Instance;
    private static AudioSource audioSource;
    private static FootSoundLibrary soundEffectLibrary;
    [SerializeField] private Slider sfxSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<FootSoundLibrary>();
        }
    }

    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if(audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}
