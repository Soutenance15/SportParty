using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CreditsDisplay : MonoBehaviour
{
    [System.Serializable]
    public class CreditItem
    {
        public enum CreditType { Text, Image }
        public CreditType type = CreditType.Text;

        [Header("Texte")]
        [TextArea(2, 5)]
        public string text;

        [Header("Image")]
        public Sprite image;
        public float imageWidth = 400f;
        public float imageHeight = 200f;
    }

    [Header("UI")]
    public TMP_Text creditText;
    public Image creditImage;

    [Header("Crédits Séquentiels")]
    public List<CreditItem> credits = new List<CreditItem>();

    [Header("Réglages")]
    public float fadeDuration = 1f;
    public float displayDuration = 2.5f;

    [Header("Audio")]
    public AudioClip creditsMusic;
    private AudioSource audioSource;
    private float musicVolume = 0.5f;

    // Sauvegarde les valeurs initiales
    private Vector2 baseImageSize;
    private Vector3 baseImageScale;
    private Vector2 baseImagePos;

    private void Start()
    {
        // Récupère le volume depuis PlayerPrefs (ou valeur par défaut)
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        // Prépare l’audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = musicVolume;

        if (creditsMusic != null)
            audioSource.clip = creditsMusic;

        // Sauvegarde la taille et la position d’origine
        baseImageSize = creditImage.rectTransform.sizeDelta;
        baseImageScale = creditImage.rectTransform.localScale;
        baseImagePos = creditImage.rectTransform.anchoredPosition;

        if (credits.Count > 0)
            StartCoroutine(DisplayCreditsSequence());
        else
            Debug.LogWarning("⚠️ Aucun élément de crédits à afficher !");
    }

    private IEnumerator DisplayCreditsSequence()
    {
        SetAlpha(creditText, 0);
        SetAlpha(creditImage, 0);

        // 🎵 Lance la musique
        if (creditsMusic != null)
            audioSource.Play();

        foreach (var item in credits)
        {
            creditText.text = "";
            creditImage.sprite = null;

            creditImage.rectTransform.sizeDelta = baseImageSize;
            creditImage.rectTransform.localScale = baseImageScale;
            creditImage.rectTransform.anchoredPosition = baseImagePos;

            if (item.type == CreditItem.CreditType.Text)
            {
                creditText.text = item.text;
                yield return StartCoroutine(FadeUI(creditText, 1f));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeUI(creditText, 0f));
            }
            else if (item.type == CreditItem.CreditType.Image && item.image != null)
            {
                creditImage.sprite = item.image;
                creditImage.rectTransform.sizeDelta = new Vector2(item.imageWidth, item.imageHeight);
                yield return StartCoroutine(FadeUI(creditImage, 1f));
                yield return new WaitForSeconds(displayDuration);
                yield return StartCoroutine(FadeUI(creditImage, 0f));
            }
        }

        // 🎚️ Fade-out progressif du son avant retour menu
        yield return StartCoroutine(FadeOutMusic(1.5f));

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FadeUI(Graphic uiElement, float targetAlpha)
    {
        if (uiElement == null) yield break;

        Color c = uiElement.color;
        float startAlpha = c.a;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            uiElement.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        uiElement.color = c;
    }

    private void SetAlpha(Graphic uiElement, float alpha)
    {
        if (uiElement == null) return;
        Color c = uiElement.color;
        c.a = alpha;
        uiElement.color = c;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        if (audioSource == null || !audioSource.isPlaying)
            yield break;

        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        audioSource.Stop();
    }
}
