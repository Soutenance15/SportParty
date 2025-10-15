using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    public float initialSpeed = 6f;
    public float speedIncrease = 0.5f;
    public float maxSpeed = 15f;
    public float flashDuration = 0.6f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitPaddleSound; // 🎵 Son joué quand la balle touche une raquette
    public AudioClip goalSound;      // 🎯 Son joué quand la balle entre dans un goal

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

        // 🎧 Initialise la source audio si elle n’existe pas encore
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.8f;
        }

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

        sr.color = baseColor;

        StartCoroutine(FlashBeforeLaunch(launchRight));
    }

    private System.Collections.IEnumerator FlashBeforeLaunch(bool launchRight)
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
        if (isScoring) return;

        if (collision.gameObject.CompareTag("PlayerLeft") || collision.gameObject.CompareTag("PlayerRight"))
        {
            // 🎧 Joue le son du rebond
            if (hitPaddleSound != null)
            {
                GameObject tempGO = new GameObject("TempAudio_Hit");
                AudioSource aSource = tempGO.AddComponent<AudioSource>();
                aSource.clip = hitPaddleSound;
                aSource.volume = 0.9f;
                aSource.spatialBlend = 0f; // 2D pur
                aSource.priority = 0;
                aSource.Play();
                Destroy(tempGO, hitPaddleSound.length);
            }

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
            isScoring = true;

            // 🎯 Joue le son de goal
            if (goalSound != null)
            {
                GameObject tempGO = new GameObject("TempAudio_Goal");
                AudioSource aSource = tempGO.AddComponent<AudioSource>();
                aSource.clip = goalSound;
                aSource.volume = 0.9f;
                aSource.spatialBlend = 0f; // 2D pur
                aSource.priority = 0;
                aSource.Play();
                Destroy(tempGO, goalSound.length);
            }

            // ⚡ Effet visuel : flash du mur d'énergie gauche
            if (GameManager_PingPong.Instance.energyWallLeft != null)
                GameManager_PingPong.Instance.energyWallLeft.BurstColor();

            GameManager_PingPong.Instance.GoalScored(leftPlayerLost: true);
        }
        else if (other.CompareTag("GoalRight"))
        {
            isScoring = true;

            // 🎯 Joue le son de goal
            if (goalSound != null)
            {
                GameObject tempGO = new GameObject("TempAudio_Goal");
                AudioSource aSource = tempGO.AddComponent<AudioSource>();
                aSource.clip = goalSound;
                aSource.volume = 0.9f;
                aSource.spatialBlend = 0f; // 2D pur
                aSource.priority = 0;
                aSource.Play();
                Destroy(tempGO, goalSound.length);
            }

            // ⚡ Effet visuel : flash du mur d'énergie droit
            if (GameManager_PingPong.Instance.energyWallRight != null)
                GameManager_PingPong.Instance.energyWallRight.BurstColor();

            GameManager_PingPong.Instance.GoalScored(leftPlayerLost: false);
        }
    }
}
