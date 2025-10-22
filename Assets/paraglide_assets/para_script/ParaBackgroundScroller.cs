using UnityEngine;

public class ParaBackgroundScroller : MonoBehaviour
{
    private float gameDuration;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timeElapsed = 0f;

    void Start()
    {
        // On récupère la durée de la partie depuis le Score Manager
        ParaScoreManager scoreManager = FindFirstObjectByType<ParaScoreManager>();
        if (scoreManager != null)
        {
            gameDuration = scoreManager.gameDuration;
        }
        else
        {
            Debug.LogError("ParaScoreManager non trouvé ! Le fond ne pourra pas défiler.");
            enabled = false; // On désactive ce script s'il ne trouve pas le manager
            return;
        }

        // --- Calcul des positions de départ et de fin ---
        Camera mainCamera = Camera.main;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = mainCamera.orthographicSize;
        float spriteHeight = spriteRenderer.bounds.size.y;

        // Position de départ : on aligne le HAUT de l'image avec le HAUT de la caméra
        startPosition = new Vector3(transform.position.x, cameraHeight - (spriteHeight / 2f), transform.position.z);

        // Position de fin : on aligne le BAS de l'image avec le BAS de la caméra
        endPosition = new Vector3(transform.position.x, -cameraHeight + (spriteHeight / 2f), transform.position.z);
        
        // On place l'image à sa position de départ
        transform.position = startPosition;
    }

    void Update()
    {
        // On ne défile que si la partie n'est pas finie
        if (timeElapsed < gameDuration)
        {
            // On calcule la progression du temps (de 0 à 1)
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / gameDuration;

            // On déplace l'image doucement de la position de départ à la position de fin
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
        }
    }
}