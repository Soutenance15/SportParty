using TMPro;
using UnityEngine;

public class NextStepManager : MonoBehaviour
{
    GameObject TutoUI;
    TextMeshProUGUI indicationText;
    TextMeshProUGUI playerNameText;
    public TextMeshProUGUI isReadyText;
    public TextMeshProUGUI nextEtapeText;
    public TextMeshProUGUI instructionStart;

    public int currentIndexStep = 0;

    public string[] steps;

    public void AssignUI(int index)
    {
        if (index == 0)
        {
            TutoUI = GameObject.Find("UI_1").transform.Find("Tuto_UI").gameObject;
        }
        else if (index == 1)
        {
            TutoUI = GameObject.Find("UI_2").transform.Find("Tuto_UI").gameObject;
        }
        if (null != TutoUI)
        {
            playerNameText = TutoUI
                .transform.Find("PlayerNameText")
                .GetComponent<TextMeshProUGUI>();

            nextEtapeText = TutoUI
                .transform.Find("Block")
                .transform.Find("NextEtapeText")
                .GetComponent<TextMeshProUGUI>();

            indicationText = TutoUI
                .transform.Find("Block")
                .transform.Find("indicationText")
                .GetComponent<TextMeshProUGUI>();
            isReadyText = TutoUI
                .transform.Find("Block")
                .transform.Find("IsReadyText")
                .GetComponent<TextMeshProUGUI>();

            instructionStart = TutoUI
                .transform.Find("Block")
                .transform.Find("InstructionStart")
                .GetComponent<TextMeshProUGUI>();
        }
        CreateStep();
        if (null != TutoUI)
        {
            indicationText.text = steps[currentIndexStep];
            isReadyText.text = "";
            if (index == 0 && null != playerNameText)
            {
                playerNameText.text = GameDataManager.Player1;
            }
            if (index == 1 && null != playerNameText)
            {
                playerNameText.text = GameDataManager.Player2;
            }
            if (null != nextEtapeText)
            {
                nextEtapeText.text = "Appuyer sur Y (Pavé Haut) pour prochaine instruction";
            }
            if (null != instructionStart)
            {
                instructionStart.text = "Appuyer sur Start quand vous êtes prêt";
            }
        }
    }

    public void CreateStep()
    {
        steps = new string[5]; // crée un tableau de 5 éléments
        steps[0] = "Droite - Gauche pour tourner";
        steps[1] = "Maintenir A ou Haut pour accélerer";
        steps[2] = "Maintienir B ou Bas pour Freiner/Reculer";
        steps[3] = "Appuie sur Select / Espace pour changer de peinture";
        steps[4] = "Les balles roses sont vos amies!";
    }

    public void NextStep()
    {
        currentIndexStep += 1;
        if (currentIndexStep > steps.Length - 1)
        {
            currentIndexStep = 0;
        }
        if (null != indicationText)
        {
            indicationText.text = steps[currentIndexStep];
        }
    }

    public void ShowTutoUI(bool show)
    {
        TutoUI.transform.Find("Block").GetComponent<GameObject>().SetActive(show);
    }
}
