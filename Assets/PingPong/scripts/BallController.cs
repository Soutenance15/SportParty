using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    public float initialSpeed = 6f;
    public float speedIncrease = 0.5f;
    public float maxSpeed = 15f;
    public float flashDuration = 0.6f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitPaddleSound;
    public AudioClip goalSound;
    public AudioClip wallBounceSound; // ✅ Son rebond sur limite

    [Header("Visual Feedback")]
    public Color normalTrailColor = Color.cyan;
    public Color intenseTrailColor = Color.magenta;
    public Color flashColor = Color.white;

    [Header("Particle Trail (Poussière d’étoiles)")]
    public int rallyForFirstTrail = 3;
    public int rallyForSecondTrail = 6;
    public float particleSpeedLow = 0.2f;
    public float particleSpeedHigh = 0.5f;
    public float particleSizeLow = 0.15f;
    public float particleSizeHigh = 0.25f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private TrailRenderer trail;
    private ParticleSystem trailParticles;
    private ParticleSystem.MainModule particleMain;

    private Color baseColor;
    private bool initialized = false;
    private bool isScoring = false;

    private int rallyCount = 0;
    public static int rallyCountGlobal = 0;
    private int currentTrailLevel = 0;
    private Coroutine flashRoutine;

    private Vector3 originalScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
        trailParticles = GetComponent<ParticleSystem>();

        if (sr != null)
            baseColor = sr.color;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.8f;
        }

        // ⚙️ Setup du TrailRenderer
        if (trail != null)
        {
            trail.enabled = false;
            trail.time = 0.25f;
            trail.startWidth = 0.2f;
            trail.endWidth = 0f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = normalTrailColor;
            trail.endColor = new Color(normalTrailColor.r, normalTrailColor.g, normalTrailColor.b, 0);
        }

        // 🌌 Setup du Particle System (poussière d’étoiles)
        if (trailParticles != null)
        {
            particleMain = trailParticles.main;
            particleMain.simulationSpace = ParticleSystemSimulationSpace.World;
            particleMain.startSpeed = 0f;
            particleMain.startSize = 0.15f;
            particleMain.startLifetime = 0.4f;
            trailParticles.Stop();
        }

        originalScale = transform.localScale;
        initialized = true;
    }

    public void LaunchBall(bool launchRight = true)
    {
        if (!initialized || rb == null) return;

        float xDir = launchRight ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector2(xDir, yDir).normalized * initialSpeed;

        rallyCount = 0;
        rallyCountGlobal = 0;
        currentTrailLevel = 0;
        transform.localScale = originalScale;

        if (trail != null) trail.enabled = false;
        if (trailParticles != null) trailParticles.Stop();

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        sr.color = baseColor;
    }

    public void ResetBall(bool launchRight)
    {
        isScoring = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = Vector2.zero;
        transform.localScale = originalScale;
        sr.color = baseColor;
        rallyCount = 0;
        rallyCountGlobal = 0;
        currentTrailLevel = 0;

        if (trail != null) trail.enabled = false;
        if (trailParticles != null) trailParticles.Stop();

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

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
        if (isScoring) return;

        // ✅ Rebond sur les raquettes
        if (collision.gameObject.CompareTag("PlayerLeft") || collision.gameObject.CompareTag("PlayerRight"))
        {
            rallyCount++;
            rallyCountGlobal = rallyCount;
            UpdateTrailEffect(collision.gameObject.CompareTag("PlayerLeft"));

            if (hitPaddleSound != null)
                PlayTempSound(hitPaddleSound, 0.9f);

            float randomY = Random.Range(-0.3f, 0.3f);
            Vector2 dir = rb.linearVelocity.normalized;
            dir.y += randomY;
            rb.linearVelocity = dir.normalized * Mathf.Min(rb.linearVelocity.magnitude + speedIncrease, maxSpeed);
        }
        // ✅ Rebond sur les limites haut/bas
        else if (collision.gameObject.CompareTag("Limite"))
        {
            if (wallBounceSound != null)
                PlayTempSound(wallBounceSound, 0.6f);
        }
    }

    private void UpdateTrailEffect(bool fromLeft)
    {
        if (trail == null && trailParticles == null) return;

        int newLevel = 0;
        if (rallyCount >= rallyForSecondTrail) newLevel = 2;
        else if (rallyCount >= rallyForFirstTrail) newLevel = 1;

        if (newLevel != currentTrailLevel)
        {
            currentTrailLevel = newLevel;

            if (flashRoutine != null)
                StopCoroutine(flashRoutine);

            if (newLevel == 1)
                flashRoutine = StartCoroutine(FlashLoop(0.4f));
            else if (newLevel == 2)
                flashRoutine = StartCoroutine(FlashLoop(0.15f));
            else
                sr.color = baseColor;
        }

        // 🌈 TRAILRENDERER
        if (newLevel == 2)
        {
            trail.enabled = true;
            trail.time = 0.5f;
            trail.startWidth = 0.3f;
            trail.startColor = intenseTrailColor;
            trail.endColor = new Color(intenseTrailColor.r, intenseTrailColor.g, intenseTrailColor.b, 0);
        }
        else if (newLevel == 1)
        {
            trail.enabled = true;
            trail.time = 0.3f;
            trail.startWidth = 0.2f;
            trail.startColor = fromLeft ? Color.cyan : Color.red;
            trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0);
        }
        else
        {
            trail.enabled = false;
        }

        // 🌌 PARTICULES
        if (trailParticles != null)
        {
            if (newLevel == 0)
            {
                trailParticles.Stop();
            }
            else
            {
                if (!trailParticles.isPlaying)
                    trailParticles.Play();

                if (newLevel == 1)
                {
                    particleMain.startColor = fromLeft ? Color.cyan : Color.red;
                    particleMain.startSize = particleSizeLow;
                    particleMain.startSpeed = particleSpeedLow;
                }
                else if (newLevel == 2)
                {
                    particleMain.startColor = intenseTrailColor;
                    particleMain.startSize = particleSizeHigh;
                    particleMain.startSpeed = particleSpeedHigh;
                }
            }
        }
    }

    private IEnumerator FlashLoop(float speed)
    {
        float scaleBoost = 1.08f;
        float flashIntensity = 1.8f;
        float pulseSpeed = 2f / speed;

        while (!isScoring)
        {
            float time = 0f;
            while (time < Mathf.PI * 2f)
            {
                float pulse = (Mathf.Sin(time * pulseSpeed) + 1f) / 2f;
                sr.color = Color.Lerp(baseColor, flashColor * flashIntensity, pulse);
                transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleBoost, pulse);
                time += Time.deltaTime;
                yield return null;
            }
        }

        sr.color = baseColor;
        transform.localScale = originalScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isScoring) return;

        if (other.CompareTag("GoalLeft") || other.CompareTag("GoalRight"))
        {
            isScoring = true;

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                flashRoutine = null;
            }

            sr.color = baseColor;
            transform.localScale = originalScale;

            if (goalSound != null)
                PlayTempSound(goalSound, 0.9f);

            rallyCount = 0;
            rallyCountGlobal = 0;
            currentTrailLevel = 0;
            if (trail != null) trail.enabled = false;
            if (trailParticles != null) trailParticles.Stop();

            if (other.CompareTag("GoalLeft") && GameManager_PingPong.Instance.energyWallLeft != null)
                GameManager_PingPong.Instance.energyWallLeft.BurstColor();
            else if (other.CompareTag("GoalRight") && GameManager_PingPong.Instance.energyWallRight != null)
                GameManager_PingPong.Instance.energyWallRight.BurstColor();

            GameManager_PingPong.Instance.GoalScored(leftPlayerLost: other.CompareTag("GoalRight") ? false : true);
        }
    }

    // 🎧 Méthode générique pour jouer un son temporaire
    private void PlayTempSound(AudioClip clip, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.spatialBlend = 0f;
        aSource.Play();
        Destroy(tempGO, clip.length);
    }
}
