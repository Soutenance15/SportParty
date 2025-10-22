using UnityEngine;

public class ParaRing : MonoBehaviour
{
    public int scoreValue = 100;
    [Tooltip("Le rayon du 'trou' de l'anneau. Le joueur doit être DANS ce rayon pour marquer.")]
    public float scoreRadius = 0.5f;

    private bool hasBeenTriggered = false;
    private ParaRingApproaching approachingScript;

    void Start()
    {
        approachingScript = GetComponent<ParaRingApproaching>();
    }

    // On remplace OnTriggerEnter2D par OnTriggerStay2D
    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (hasBeenTriggered || !otherCollider.CompareTag("Player"))
        {
            return;
        }

        // On calcule la distance entre le centre de l'anneau et le centre du joueur
        float distance = Vector2.Distance(transform.position, otherCollider.transform.position);

        // On vérifie si le joueur est bien DANS le "trou" de l'anneau
        if (distance <= scoreRadius)
        {
            // Si la condition est respectée, on valide le point
            hasBeenTriggered = true;

            ParaPlayerController player = otherCollider.GetComponent<ParaPlayerController>();
            if (player != null)
            {
                player.AddScore(scoreValue);
            }

            if (approachingScript != null)
            {
                approachingScript.TriggerValidationAnimation();
            }
        }
    }
}