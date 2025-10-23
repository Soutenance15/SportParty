using UnityEngine;

public class ParaRingSpawner : MonoBehaviour
{
    public GameObject ringPrefab;
    [Tooltip("Le temps en secondes entre l'apparition de chaque anneau.")]
    public float spawnInterval = 1.5f;
    public Vector3 spawnOrigin = new Vector3(0, -2, 0);
    public Vector2 horizontalBounds = new Vector2(-8f, 8f);
    public Vector2 verticalBounds = new Vector2(-1f, 4f);

    private float timer;

    void Update()
    {
        // On incrémente le compteur en continu
        timer += Time.deltaTime;

        // Si le temps est écoulé, on crée un anneau et on réinitialise.
        // C'est la seule logique.
        if (timer >= spawnInterval)
        {
            SpawnRing();
            timer = 0f;
        }
    }

    void SpawnRing()
    {
        float randomX = Random.Range(horizontalBounds.x, horizontalBounds.y);
        float randomY = Random.Range(verticalBounds.x, verticalBounds.y);
        Vector3 targetPos = new Vector3(randomX, randomY, 0);

        GameObject newRing = Instantiate(ringPrefab, spawnOrigin, Quaternion.identity);
        newRing.GetComponent<ParaRingApproaching>().Initialize(targetPos);
    }
}