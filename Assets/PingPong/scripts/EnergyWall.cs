using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnergyWall : MonoBehaviour
{
    [Header("Base Settings")]
    public Color baseColor = Color.cyan;
    public float pulseSpeed = 2f;
    public float alphaMin = 0.4f;
    public float alphaMax = 0.8f;

    [Header("Proximity FX")]
    public float proximityRange = 2.5f;
    public float proximityGlowBoost = 1.5f;
    public float proximityPulseSpeed = 4f;

    [Header("Touch FX")]
    public float flashDuration = 0.2f;
    public Color flashColor = Color.white;

    private SpriteRenderer sr;
    private Color currentColor;
    private float flashTimer = 0f;
    private Transform playerLeft;
    private Transform playerRight;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentColor = baseColor;

        // Cherche les joueurs
        GameObject left = GameObject.FindWithTag("PlayerLeft");
        GameObject right = GameObject.FindWithTag("PlayerRight");

        if (left != null) playerLeft = left.transform;
        if (right != null) playerRight = right.transform;
    }

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * pulseSpeed, alphaMax - alphaMin) + alphaMin;
        Color targetColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

        // Effet de proximité
        float proximityBoost = 1f;
        if (playerLeft != null && Vector2.Distance(transform.position, playerLeft.position) < proximityRange)
            proximityBoost += proximityGlowBoost;

        if (playerRight != null && Vector2.Distance(transform.position, playerRight.position) < proximityRange)
            proximityBoost += proximityGlowBoost;

        // Si flash en cours
        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            targetColor = Color.Lerp(flashColor, baseColor, flashTimer / flashDuration);
        }

        // Application
        targetColor *= proximityBoost;
        sr.color = Color.Lerp(sr.color, targetColor, Time.deltaTime * proximityPulseSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            flashTimer = flashDuration;
            Debug.Log("💥 Raquette a touché le mur d'énergie !");
        }
    }
    public void BurstColor()
{
    StartCoroutine(BurstEffect());
}

private IEnumerator BurstEffect()
{
    Color original = sr.color;
    for (float t = 0; t < 0.5f; t += Time.deltaTime)
    {
        float flash = Mathf.PingPong(Time.time * 20f, 1f);
        sr.color = Color.Lerp(original, Color.white, flash);
        yield return null;
    }
    sr.color = original;
}

}
