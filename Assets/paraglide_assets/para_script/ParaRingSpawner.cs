using UnityEngine;
using System.Collections;

public class ParaRingSpawner : MonoBehaviour
{
    public GameObject ringPrefab;
    public float spawnInterval = 2.0f; // Un peu plus long pour éviter la superposition

    [Header("Positions")]
    [Tooltip("Le point d'origine où les anneaux apparaissent (avant de se déplacer)")]
    public Vector3 spawnOrigin = Vector3.zero; // <<< VARIABLE AJOUTÉE
    public Vector2 horizontalBounds = new Vector2(-8f, 8f);
    public Vector2 verticalBounds = new Vector2(-5f, 2f);
    // Compteur de temps
    private float timer = 0f;

    // Référence à l'anneau en jeu
    private static GameObject currentActiveRing;

    void Update()
    {
        // // Si un anneau existe déjà, on ne fait rien.
        // if (currentActiveRing != null)
        // {
        //     return;
        // }

        // Si aucun anneau n'existe, on commence le décompte.
        timer += Time.deltaTime;

        // Si le temps est écoulé, on fait apparaître un anneau et on réinitialise le compteur.
        if (timer >= spawnInterval)
        {
            Debug.Log(timer);
            SpawnRing();
            timer = 0f;
        }
    }

    void SpawnRing()
    {
        float randomX = Random.Range(-8f, 8f);
        float randomY = Random.Range(-5f, 2f);
        Vector3 targetPos = new Vector3(randomX, randomY, 0);

        GameObject newRing = Instantiate(ringPrefab, spawnOrigin, Quaternion.identity); // <<< LIGNE MODIFIÉE
        newRing.GetComponent<ParaRingApproaching>().Initialize(targetPos);
    }
}