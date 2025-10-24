using UnityEngine;

public class ParaLeafBehavior : MonoBehaviour
{
    [Header("Général")]
    public float lifetime = 6f;         // Durée de vie de la feuille
    public float startScale = 0.1f;     // Taille au début
    public float endScale = 1.0f;       // Taille à la fin

    [Header("Chute et Avancée")]
    public float fallSpeed = 0.5f;      // Vitesse moyenne de chute
    public float approachSpeed = 1.0f;  // Vitesse d'approche vers la caméra (augmente l'échelle)
    public float horizontalDrift = 0.2f; // Dérive horizontale lente

    [Header("Spirale et Rotation")]
    public float swirlAmplitude = 0.8f; // Amplitude du mouvement de spirale latérale
    public float swirlFrequency = 2.0f; // Fréquence de la spirale (plus grand = spirale serrée)
    public float rotationSpeed = 180f;  // Vitesse de rotation de la feuille sur elle-même (en degrés/sec)
    
    private float timer;
    private Vector3 initialPosition; // Position de spawn
    private float randomOffset;      // Pour décaler la spirale de chaque feuille
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        timer = 0f;
        initialPosition = transform.position;
        transform.localScale = Vector3.one * startScale;
        randomOffset = Random.Range(0f, 100f);
        spriteRenderer = GetComponent<SpriteRenderer>();

        // --- MODIFICATION ICI ---
        // S'assurer que la couleur de départ est totalement opaque
        if (spriteRenderer != null) spriteRenderer.color = Color.white; // Met la couleur à blanc opaque

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / lifetime; // Progression de 0 à 1 sur la durée de vie

        if (progress < 1f)
        {
            // --- 1. Mouvement Vertical (Chute) ---
            // Simule la chute et l'approche. La position Y diminue, la position Z simule l'approche (ou est gérée par l'échelle)
            Vector3 currentPosition = initialPosition;
            currentPosition.y -= fallSpeed * timer; // Chute régulière

            // --- 2. Mouvement Horizontal (Spirale et Dérive) ---
            // Mouvement de spirale latérale (sinusoïdal)
            float swirlX = Mathf.Sin((timer + randomOffset) * swirlFrequency) * swirlAmplitude;
            float swirlY = Mathf.Cos((timer + randomOffset) * swirlFrequency * 0.5f) * swirlAmplitude * 0.5f; // Un peu de mouvement vertical aussi pour la spirale

            currentPosition.x = initialPosition.x + swirlX + (horizontalDrift * timer);
            currentPosition.y += swirlY; // Applique la composante Y de la spirale
            
            transform.position = currentPosition;

            // --- 3. Grossissement (Approche de la caméra) ---
            transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, progress);
            
            // --- 4. Rotation de la feuille sur elle-même ---
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);

        }
        else
        {
            Destroy(gameObject); // Détruire la feuille après sa durée de vie
        }
    }
}