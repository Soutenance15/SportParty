using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro; // ✅ TextMeshPro support

public class PlayerController_PingPong : MonoBehaviour
{
    [Header("Mouvement")]
    public float speed = 10f;
    public bool isLeftPlayer = true;

    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 2f;

    [Header("Audio")]
    public AudioClip dashSound;
    private AudioSource audioSource;

    [Header("Feedback Visuel")]
    public SpriteRenderer sr;
    public Color dashReadyColor = Color.white;
    public float blinkSpeed = 0.1f;
    private Coroutine blinkRoutine;

    [Header("UI Feedback (TextMeshPro)")]
    public TMP_Text dashReadyText; // ✅ TextMeshPro référence
    public Color dashTextColor = new Color(0.1f, 0.9f, 1f); // 💡 Bleu néon
    public float textBlinkSpeed = 0.1f; // clignotement rapide du texte
    private Coroutine textBlinkRoutine;

    private bool canDash = false;
    private bool isDashing = false;
    private bool dashReadyVisualActive = false;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("⚠️ Rigidbody2D manquant sur le paddle !");

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (dashReadyText != null)
        {
            dashReadyText.enabled = false; // masqué au départ
            dashReadyText.color = dashTextColor;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            moveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            moveInput = Vector2.zero;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            if (dashSound != null && audioSource != null)
                audioSource.PlayOneShot(dashSound, 0.8f);

            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        float originalSpeed = speed;
        speed = dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        speed = originalSpeed;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = BallController.rallyCountGlobal >= 5;
    }

    void FixedUpdate()
    {
        Vector2 movement = moveInput.normalized * speed * Time.fixedDeltaTime;
        Vector2 newPos = rb.position + movement;

        if (isLeftPlayer)
            newPos.x = Mathf.Clamp(newPos.x, -7f, -0.5f);
        else
            newPos.x = Mathf.Clamp(newPos.x, 0.5f, 7f);

        newPos.y = Mathf.Clamp(newPos.y, -4.5f, 4.5f);
        rb.MovePosition(newPos);
    }

    void Update()
    {
        bool dashAvailable = BallController.rallyCountGlobal >= 3 && !isDashing;

        if (dashAvailable && !dashReadyVisualActive)
        {
            canDash = true;
            dashReadyVisualActive = true;

            // ⚡ Clignotement du sprite
            if (blinkRoutine != null)
                StopCoroutine(blinkRoutine);
            blinkRoutine = StartCoroutine(BlinkEffect());

            // ⚡ Clignotement du texte TMP
            if (dashReadyText != null)
            {
                dashReadyText.text = "DASH READY!";
                dashReadyText.enabled = true;
                if (textBlinkRoutine != null)
                    StopCoroutine(textBlinkRoutine);
                textBlinkRoutine = StartCoroutine(TextBlink());
            }
        }
        else if (!dashAvailable && dashReadyVisualActive)
        {
            canDash = false;
            dashReadyVisualActive = false;

            // Stop clignotement visuel
            if (blinkRoutine != null)
                StopCoroutine(blinkRoutine);
            if (sr != null)
                sr.color = Color.white;

            // Stop texte TMP
            if (dashReadyText != null)
            {
                dashReadyText.enabled = false;
                if (textBlinkRoutine != null)
                    StopCoroutine(textBlinkRoutine);
            }
        }
    }

    // 🌈 Clignotement rapide du sprite
    private IEnumerator BlinkEffect()
    {
        while (dashReadyVisualActive)
        {
            sr.color = dashReadyColor;
            yield return new WaitForSeconds(blinkSpeed);
            sr.color = Color.white;
            yield return new WaitForSeconds(blinkSpeed);
        }
        sr.color = Color.white;
    }

    // 💡 Clignotement rapide du texte TMP
    private IEnumerator TextBlink()
    {
        Color baseColor = dashTextColor;
        while (dashReadyVisualActive)
        {
            dashReadyText.color = baseColor;
            yield return new WaitForSeconds(textBlinkSpeed);
            dashReadyText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.2f);
            yield return new WaitForSeconds(textBlinkSpeed);
        }
        dashReadyText.color = baseColor;
    }
}
