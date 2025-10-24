using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

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

    [Header("Impact Visuel")]
    public Color impactFlashColor = new Color(1f, 0.8f, 0.4f);
    public float flashDuration = 0.08f;
    public bool enableCameraShake = true;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.1f;
    private Camera mainCam;
    private Color originalColor;

    [Header("UI Feedback (TextMeshPro)")]
    public TMP_Text dashReadyText;
    public Color dashTextColor = new Color(0.1f, 0.9f, 1f);
    public float textBlinkSpeed = 0.1f;
    private Coroutine textBlinkRoutine;

    private bool canDash = false;
    private bool isDashing = false;
    private bool dashReadyVisualActive = false;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        mainCam = Camera.main;

        if (dashReadyText != null)
        {
            dashReadyText.enabled = false;
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
            if (dashSound != null)
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

            if (dashReadyText != null)
            {
                dashReadyText.enabled = false;
                if (textBlinkRoutine != null)
                    StopCoroutine(textBlinkRoutine);
            }
        }
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

    // 💥 Impact visuel quand la balle touche le paddle
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            StartCoroutine(ImpactFlash());

            if (enableCameraShake && mainCam != null)
                StartCoroutine(CameraShake());
        }
    }

    // 🌈 Flash rapide du paddle
    private IEnumerator ImpactFlash()
    {
        sr.color = impactFlashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    // 🎥 Mini secousse caméra
    private IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCam.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            mainCam.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.position = originalPos;
    }
}
