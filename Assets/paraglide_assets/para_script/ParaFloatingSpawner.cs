using UnityEngine;

public class ParaFloatingSpawner : MonoBehaviour
{
    [Header("Nuages")]
    public GameObject cloudPrefab;
    public float cloudSpawnInterval = 5f;
    public Vector2 cloudSpawnYRange = new Vector2(3f, 5f);

    [Header("Feuilles")]
    public GameObject leafPrefab;
    public float leafSpawnInterval = 1f;
    public Vector2 leafSpawnYRange = new Vector2(4f, 6f);

    [Header("Zone Horizontale (Commun)")]
    public Vector2 spawnXRange = new Vector2(-10f, 10f);

    private float cloudTimer;
    private float leafTimer;
    private ParaScoreManager scoreManager; // Référence au manager de score

    void Start()
    {
        // On cherche le Score Manager au démarrage
        scoreManager = ParaScoreManager.Instance;
        if (scoreManager == null)
        {
            Debug.LogError("ParaScoreManager non trouvé ! Le spawn de nuages/feuilles ne fonctionnera pas correctement.");
            enabled = false; // Désactive ce script si le manager n'est pas là
        }
    }

    void Update()
    {
        if (scoreManager == null) return; // Sécurité

        // On récupère la progression actuelle du jeu (0 = début, 1 = fin)
        float gameProgress = scoreManager.GetGameProgress();

        // --- Spawn des Nuages (Première Moitié) ---
        if (gameProgress < 0.5f) // Si on est dans la première moitié
        {
            cloudTimer += Time.deltaTime;
            if (cloudTimer >= cloudSpawnInterval)
            {
                SpawnObject(cloudPrefab, cloudSpawnYRange);
                cloudTimer = 0f;
            }
        }

        // --- Spawn des Feuilles (Seconde Moitié) ---
        if (gameProgress >= 0.5f) // Si on est dans la seconde moitié
        {
            leafTimer += Time.deltaTime;
            if (leafTimer >= leafSpawnInterval)
            {
                SpawnObject(leafPrefab, leafSpawnYRange);
                leafTimer = 0f;
            }
        }
    }

    void SpawnObject(GameObject prefab, Vector2 yRange)
    {
        float spawnX = (Random.value < 0.5f) ? spawnXRange.x : spawnXRange.y;
        float spawnY = Random.Range(yRange.x, yRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}