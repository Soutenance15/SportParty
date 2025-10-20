using UnityEngine;
using System.Collections;

public class AudioFader : MonoBehaviour
{
    public static AudioFader Instance;
    private AudioSource musicSource;

    private void Awake()
    {
        Instance = this;
        musicSource = GetComponent<AudioSource>();
    }

    public void FadeOut(float fadeDuration = 1.5f)
    {
        if (musicSource != null)
            StartCoroutine(FadeOutRoutine(fadeDuration));
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = musicSource.volume;

        float time = 0f;
        while (time < duration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }
}
