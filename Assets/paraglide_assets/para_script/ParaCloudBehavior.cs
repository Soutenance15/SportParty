using UnityEngine;

public class ParaCloudBehavior : MonoBehaviour
{
    [Header("Apparence")]
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    [Range(0f, 1f)] public float startAlpha = 0.8f;
    [Range(0f, 1f)] public float endAlpha = 0.1f;

    [Header("Mouvement")]
    public float lifetime = 10f; // Durée de vie plus longue pour un mouvement lent
    [Tooltip("Force constante qui pousse le nuage vers le haut")]
    public float upwardForce = 0.5f;
    [Tooltip("Vitesse de la dérive latérale")]
    public float horizontalDriftSpeed = 0.2f;

    // --- Variables privées ---
    private float timer;
    private float targetScale;
    private SpriteRenderer spriteRenderer;
    private float driftDirection; // -1 pour gauche, 1 pour droite

    void Start()
    {
        timer = 0f;
        targetScale = Random.Range(minScale, maxScale);
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Détermine la direction de la dérive (vers l'extérieur)
        driftDirection = Mathf.Sign(transform.position.x);
        // Si le nuage est exactement au centre (x=0), on choisit une direction au hasard
        if (driftDirection == 0) driftDirection = (Random.value < 0.5f) ? -1f : 1f;

        transform.localScale = Vector3.one * 0.1f; // Commence petit

        // Appliquer l'alpha de départ
        if (spriteRenderer != null)
        {
            Color startColor = spriteRenderer.color;
            startColor.a = startAlpha;
            spriteRenderer.color = startColor;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < lifetime)
        {
            float progress = timer / lifetime;

            // --- NOUVEAU MOUVEMENT ---
            // 1. Appliquer la force constante vers le haut
            transform.Translate(Vector3.up * upwardForce * Time.deltaTime, Space.World);
            // 2. Appliquer la dérive latérale
            transform.Translate(Vector3.right * driftDirection * horizontalDriftSpeed * Time.deltaTime, Space.World);

            // Grossissement
            transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one * targetScale, progress);

            // Animer l'alpha
            if (spriteRenderer != null)
            {
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(startAlpha, endAlpha, progress);
                spriteRenderer.color = currentColor;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}