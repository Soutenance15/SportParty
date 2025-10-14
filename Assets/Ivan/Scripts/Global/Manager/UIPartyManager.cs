using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyManager : MonoBehaviour
{
    [SerializeField]
    private Button kartButton;

    [SerializeField]
    private Button footButton;

    private PartyManager partyManager;

    public event Action OnKartPressed;
    public event Action OnFootPressed;

    void Start()
    {
        // Récupère le PartyManager dans la scène
        partyManager = FindFirstObjectByType<PartyManager>();

        // Vérifie que tout est bien configuré
        if (partyManager == null)
        {
            Debug.LogError("❌ Aucun PartyManager trouvé dans la scène !");
            return;
        }

        if (kartButton != null)
            kartButton.onClick.AddListener(() => KartPressed());

        if (footButton != null)
            footButton.onClick.AddListener(() => FootPressed());
    }

    void KartPressed()
    {
        OnKartPressed?.Invoke();
        // Debug.Log($"🕹️ Mini-jeu choisi : {choice}");
        // partyManager.SelectMiniGame(choice);
    }

    void FootPressed()
    {
        OnFootPressed?.Invoke();
        // Debug.Log($"🕹️ Mini-jeu choisi : {choice}");
        // partyManager.SelectMiniGame(choice);
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
