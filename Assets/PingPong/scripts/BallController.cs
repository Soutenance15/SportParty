using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    public float initialSpeed = 6f;
    public float speedIncrease = 0.5f;
    public float maxSpeed = 15f;
    public float flashDuration = 0.6f;

    [Header("Goal Effect")]
    public float fadeOutDuration = 0.4f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 direction;
    private Color baseColor;
    private bool initialized = false;
    private bool isScoring = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (rb == null)
            Debug.LogError("❌ Aucun Rigidbody2D trouvé sur la balle !");
        if (sr == null)
            Debug.LogError("❌ Aucun SpriteRenderer trouvé sur la balle !");
        else
            baseColor = sr.color;

        initialized = true;
    }

    public void LaunchBall(bool launchRight = true)
    {
        if (!initialized || rb == null)
        {
            Debug.LogWarning("⚠️ Ball non initialisée, lancement ignoré.");
            return;
        }

        float xDir = launchRight ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);
        direction = new Vector2(xDir, yDir).normalized;

        rb.velocity = direction * initialSpeed;
        Debug.Log($"🚀 Balle lancée vers {(launchRight ? "droite" : "gauche")}");
    }

    public void ResetBall(bool launchRight)
    {
        if (rb == null) return;

        isScoring = false;
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;

        // Restaure la visibilité et couleur de base
        sr.color = baseColor;

        // Flash lumineux avant relance
        StartCoroutine(FlashBeforeLaunch(launchRight));
    }

    private IEnumerator FlashBeforeLaunch(bool launchRight)
    {
        float timer = 0f;
        while (timer < flashDuration)
        {
            float alpha = Mathf.PingPong(Time.time * 8f, 1f);
            sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        sr.color = baseColor;
        LaunchBall(launchRight);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isScoring) return; // ignore les collisions si la balle “meurt”

        if (collision.gameObject.CompareTag("PlayerLeft") || collision.gameObject.CompareTag("PlayerRight"))
        {
            float randomY = Random.Range(-0.3f, 0.3f);
            Vector2 dir = rb.velocity.normalized;
            dir.y += randomY;
            rb.velocity = dir.normalized * Mathf.Min(rb.velocity.magnitude + speedIncrease, maxSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isScoring) return;

        if (other.CompareTag("GoalLeft"))
        {
            StartCoroutine(HandleGoal(true));
        }
        else if (other.CompareTag("GoalRight"))
        {
            StartCoroutine(HandleGoal(false));
        }
    }

    private IEnumerator HandleGoal(bool leftPlayerLost)
    {
        isScoring = true;
        rb.velocity = Vector2.zero;

        // Effet de disparition simple (fade-out)
        float timer = 0f;
        Color startColor = sr.color;

        while (timer < fadeOutDuration)
        {
            float t = timer / fadeOutDuration;
            sr.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);
            timer += Time.deltaTime;
            yield return null;
        }

        // Cache complètement la balle
        sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Informe le GameManager
        GameManager_PingPong.Instance.GoalScored(leftPlayerLost);
    }
}
