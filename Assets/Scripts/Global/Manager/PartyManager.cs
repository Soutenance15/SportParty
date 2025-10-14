using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public bool hasWinner;
    private Party currentParty;
    public int nbPlayerParty = 2;
    GameObject miniGameGO;
    string minigameChoice;

    public void UpdateParty()
    {
        // Setup Party
        if (currentParty == null || !currentParty.isRunning)
        {
            SetupParty();
        }

        // Continue Party
        if (currentParty != null && currentParty.miniGame != null)
        {
            // Simule Victoire
            if (Input.GetKeyDown(KeyCode.U))
            {
                currentParty.miniGame.WinMiniGame();
            }
        }
    }

    private void SetupParty()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Create party
            if (currentParty == null)
            {
                currentParty = new Party();
                Debug.Log("🎮 Nouvelle partie créée !");
            }
        }

        // Add player to the party
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentParty == null)
            {
                Debug.LogWarning(
                    "⚠️ Crée d'abord une partie (flèche gauche) avant d'ajouter un joueur !"
                );
                return;
            }

            string playerName = $"Joueur {currentParty.numberOfPlayers + 1}";
            if (currentParty.numberOfPlayers < nbPlayerParty)
            {
                currentParty.AddPlayer(playerName);
                Debug.Log($"👤 Joueur ajouté : {playerName}");
            }
        }

        // Type Mini Game
        AssignMiniGame();

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
                switch (minigameChoice)
                {
                    case "KartMiniGame":
                        currentParty.miniGame = miniGameGO.AddComponent<KartMiniGame>();
                        currentParty.miniGame.OnWinMiniGame += QuitCurrentParty;
                        break;
                    case "FootMiniGame":
                        currentParty.miniGame = miniGameGO.AddComponent<FootMiniGame>();
                        currentParty.miniGame.OnWinMiniGame += QuitCurrentParty;
                        break;
                }
            }
        }
    }

    bool partyCanBeLauch()
    {
        return (
            currentParty != null
            && currentParty.numberOfPlayers == nbPlayerParty
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
            currentParty = null;
        }
    }

    void QuitCurrentParty()
    {
        ResetCurrentParty();
        Debug.Log("Party Manager : Quit Current Party");
    }

    void AssignMiniGame()
    {
        if (currentParty != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                minigameChoice = "KartMiniGame";
                Debug.Log("MiniGame Choisi + Kart");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                minigameChoice = "FootMiniGame";
                Debug.Log("MiniGame Choisi + Foot");
            }
        }
    }
}
