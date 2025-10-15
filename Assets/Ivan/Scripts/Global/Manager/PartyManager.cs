using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public bool hasWinner;
    private Party currentParty;
    public int nbPlayerParty = 2;
    GameObject miniGameGO;
    string minigameChoice;

    public UIPartyManager uIPartyManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentParty = new Party();
    }

    void OnEnable()
    {
        if (uIPartyManager != null)
        {
            uIPartyManager.OnKartPressed += SelectKartMiniGame;
            uIPartyManager.OnFootPressed += SelectFootMiniGame;
        }
    }

    void OnDisable()
    {
        if (uIPartyManager != null)
        {
            uIPartyManager.OnKartPressed -= SelectKartMiniGame;
            uIPartyManager.OnFootPressed -= SelectFootMiniGame;
        }
    }

    void SelectKartMiniGame()
    {
        Debug.Log("SelectKartMiniGame");
        minigameChoice = "KartMiniGame";
    }

    void SelectFootMiniGame()
    {
        Debug.Log("SelectFootMiniGame");
        minigameChoice = "FootMiniGame";
    }

    public void Update()
    {
        // Setup Party
        if (!currentParty.isRunning)
        {
            SetupParty();
        }

        // Continue Party
        if (currentParty.miniGame != null)
        {
            // Simule Victoire
            if (Input.GetKeyDown(KeyCode.U))
            {
                currentParty.miniGame.WinMiniGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitCurrentParty();
        }
    }

    private void SetupParty()
    {
        // Start Party
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (partyCanBeLauch())
            {
                if (miniGameGO != null)
                {
                    Destroy(miniGameGO);
                }
                miniGameGO = new GameObject(minigameChoice);
                // TO DO
                // Amélioré avec nom editable, nmbr joueur à définir
                currentParty.AddPlayer("Joueur 1");
                currentParty.AddPlayer("Joueur 2");
                switch (minigameChoice)
                {
                    case "KartMiniGame":
                        currentParty.miniGame = miniGameGO.AddComponent<KartMiniGame>();
                        // currentParty.miniGame.OnWinMiniGame += QuitCurrentParty;
                        break;
                    case "FootMiniGame":
                        currentParty.miniGame = miniGameGO.AddComponent<FootMiniGame>();
                        // currentParty.miniGame.OnWinMiniGame += QuitCurrentParty;
                        break;
                }
                uIPartyManager.Show(false);
            }
        }
    }

    bool partyCanBeLauch()
    {
        return (
            currentParty != null
            // && currentParty.numberOfPlayers == nbPlayerParty
            && currentParty.miniGame == null
            && !currentParty.isRunning
        );
    }

    void ResetCurrentParty()
    {
        if (miniGameGO != null)
        {
            Destroy(miniGameGO);
        }
        if (currentParty != null)
        {
            if (currentParty.miniGame != null)
            {
                currentParty.miniGame.OnWinMiniGame -= QuitCurrentParty;
            }
            currentParty = new Party();
        }
        uIPartyManager.Show(true);
    }

    void QuitCurrentParty()
    {
        // ResetCurrentParty();
        Debug.Log("Party Manager : Quit Current Party");
    }
}
