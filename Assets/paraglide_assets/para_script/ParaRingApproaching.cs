using UnityEngine;
using System;

public class ParaRingApproaching : MonoBehaviour
{
    public static event Action OnRingDestroyed;

    [Header("Couleurs")]
    public Color startColor = Color.yellow;
    public Color endColor = Color.red;
    public Color validatedColor = Color.green;

    [Header("Animation")]
    public float lifetime = 2f;
    public float rotationSpeed = 720f;
    public float validationAnimationTime = 1.5f;
    
    [Header("Activation")]
    [Range(0f, 1f)]
    public float activationThreshold = 0.8f;
    
    private Vector3 targetPosition; // Doit être défini par Initialize
    private Vector3 startPosition;
    private float maxScale = 1f;
    private Vector3 startScale = new Vector3(0.05f, 0.05f, 0.05f);
    private float timer = 0f;
    private bool isValidated = false;
    private CircleCollider2D ringCollider;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector3 target)
    {
        targetPosition = target; // On reçoit la position cible

        startPosition = transform.position;
        
        transform.localScale = startScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ringCollider = GetComponent<CircleCollider2D>();

        if (spriteRenderer != null) {
            spriteRenderer.color = startColor; // On applique la couleur de départ
        }
        if (ringCollider != null) {
            ringCollider.enabled = false;
        }
    }

    void Update()
    {
        if (isValidated) {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            return;
        }

        if (timer < lifetime) {
            timer += Time.deltaTime;
            float progress = timer / lifetime;

            transform.localScale = Vector3.Lerp(startScale, new Vector3(maxScale, maxScale, maxScale), progress);
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress); // Cette ligne est cruciale

            if (spriteRenderer != null) {
                spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
            }

            if (progress >= activationThreshold && ringCollider != null && !ringCollider.enabled) {
                ringCollider.enabled = true;
            }
        } else {
            Destroy(gameObject);
        }
    }

    public void TriggerValidationAnimation()
    {
        isValidated = true;
        if (spriteRenderer != null) {
            spriteRenderer.color = validatedColor;
        }
        if(ringCollider != null) ringCollider.enabled = false;
        Destroy(gameObject, validationAnimationTime);
    }

    void OnDestroy()
    {
        if (OnRingDestroyed != null) {
            OnRingDestroyed();
        }
    }
}