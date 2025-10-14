using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // private bool isPartyRunning = false;
    private PartyManager partyManager; // référence vers le PartyManager dans la scène

    private void Awake()
    {
        // --- Singleton ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- Récupère le PartyManager sur le même GameObject ---
        partyManager = GetComponent<PartyManager>();

        if (partyManager == null)
        {
            Debug.LogError("⚠️ Aucun PartyManager trouvé sur le GameManager !");
        }
    }

    private void Start()
    {
        Debug.LogWarning("Aucune partie en cours. Appuie sur 'E' pour créer une nouvelle partie.");
        Debug.LogWarning("Appuie sur 'R' pour ajouter un joueur.");
        Debug.LogWarning("Appuie sur 'fleche' pour choisir jeu.");
        Debug.LogWarning("Appuie sur 'T' pour lancer la partie.");
    }

    private void Update()
    {
        // if (partyManager != null)
        // {
        //     partyManager.UpdateParty();
        // }
    }
}
