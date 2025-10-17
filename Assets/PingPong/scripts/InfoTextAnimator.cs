using UnityEngine;
using TMPro;
using System.Collections;

public class InfoTextAnimator : MonoBehaviour
{
    private TMP_Text infoText;
    private Vector3 baseScale;
    private bool isPulsing = false;

    [Header("Réglages visuels")]
    public float pulseSpeed = 2f;      // Vitesse du clignotement
    public float pulseMinAlpha = 0.4f; // Alpha minimum pendant le pulse
    public float zoomScale = 1.2f;     // Zoom au moment du tirage
    public float zoomDuration = 0.3f;  // Durée du zoom

    private void Awake()
    {
        infoText = GetComponent<TMP_Text>();
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Lance le clignotement par défaut
        StartPulse();
    }

    private void Update()
    {
        if (isPulsing)
        {
            float alpha = Mathf.Lerp(pulseMinAlpha, 1f, Mathf.PingPong(Time.time * pulseSpeed, 1f));
            Color c = infoText.color;
            c.a = alpha;
            infoText.color = c;
        }
    }

    /// <summary>
    /// Lance l'effet de clignotement
    /// </summary>
    public void StartPulse()
    {
        isPulsing = true;
    }

    /// <summary>
    /// Stoppe le clignotement et fait apparaître un effet zoom + fade
    /// </summary>
    public void ShowGameChosen()
    {
        isPulsing = false;
        StartCoroutine(AnimateSelection());
    }

    private IEnumerator AnimateSelection()
    {
        // Remet l'alpha à 1
        Color c = infoText.color;
        c.a = 1f;
        infoText.color = c;

        // Zoom avant puis retour à la normale
        float timer = 0f;
        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / zoomDuration;
            transform.localScale = Vector3.Lerp(baseScale, baseScale * zoomScale, t);
            yield return null;
        }

        // Retour
        timer = 0f;
        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;
            float t = timer / zoomDuration;
            transform.localScale = Vector3.Lerp(baseScale * zoomScale, baseScale, t);
            yield return null;
        }
    }
}
