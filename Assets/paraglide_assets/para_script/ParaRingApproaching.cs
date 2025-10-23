using UnityEngine;
using System;

public class ParaRingApproaching : MonoBehaviour
{
    public static event Action OnRingDestroyed;

    [Header("Taille et Durée")]
    public float approachTime = 3f;
    [Tooltip("La taille finale que l'anneau atteindra. Une valeur > 1 donnera l'impression qu'il traverse l'écran.")]
    public float maxScale = 4f; // <<< MODIFIÉ
    [Range(0f, 1f)] public float activationThreshold = 0.5f; // <<< RÉGLÉ À 0.5 PAR DÉFAUT

    [Header("Courbes d'Animation")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Couleurs et Validation")]
    public Color startColor = Color.yellow;
    // ... (le reste de vos variables ne change pas)
    public Color endColor = Color.red;
    public Color validatedColor = Color.green;
    public float rotationSpeed = 720f;
    public float validationAnimationTime = 1.5f;
    
    // --- Variables privées ---
    private Vector3 targetPosition, startPosition;
    private float approachTimer = 0f;
    private bool isValidated = false;
    private CircleCollider2D ringCollider;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        startPosition = transform.position;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        ringCollider = GetComponent<CircleCollider2D>();

        transform.localScale = Vector3.zero;
        if (spriteRenderer != null) spriteRenderer.color = startColor;
        if (ringCollider != null) ringCollider.enabled = false;
    }

    void Update()
    {
        if (isValidated) {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            return;
        }

        if (approachTimer < approachTime)
        {
            approachTimer += Time.deltaTime;
            float progress = approachTimer / approachTime;

            float moveProgress = moveCurve.Evaluate(progress);
            float scaleProgress = scaleCurve.Evaluate(progress);

            transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);
            
            // On utilise la nouvelle variable maxScale ici
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, scaleProgress); // <<< MODIFIÉ

            if (spriteRenderer != null) spriteRenderer.color = Color.Lerp(startColor, endColor, progress);

            if (progress >= activationThreshold && ringCollider != null && !ringCollider.enabled) {
                ringCollider.enabled = true;
            }
        }
        else {
            Destroy(gameObject);
        }
    }

    public void TriggerValidationAnimation()
    {
        isValidated = true;
        if (spriteRenderer != null) spriteRenderer.color = validatedColor;
        if(ringCollider != null) ringCollider.enabled = false;
        Destroy(gameObject, validationAnimationTime);
    }

    void OnDestroy()
    {
        if (OnRingDestroyed != null) OnRingDestroyed();
    }
}