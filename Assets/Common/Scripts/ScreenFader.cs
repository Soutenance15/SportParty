using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        // Démarre avec un fade-in (depuis noir vers transparent)
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        Color c = fadeImage.color;
        for (float t = 1; t > 0; t -= Time.deltaTime / fadeDuration)
        {
            c.a = t;
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0;
        fadeImage.color = c;
    }

    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        Color c = fadeImage.color;
        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            c.a = t;
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1;
        fadeImage.color = c;

        SceneManager.LoadScene(sceneName);
    }
}
