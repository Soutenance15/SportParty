using TMPro;
using UnityEngine;

public class NextStepManager : MonoBehaviour
{
    GameObject TutoUI;
    TextMeshProUGUI indicationText;
    public TextMeshProUGUI isReadyText;

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
            indicationText = TutoUI
                .transform.Find("indicationText")
                .GetComponent<TextMeshProUGUI>();
            isReadyText = TutoUI.transform.Find("IsReadyText").GetComponent<TextMeshProUGUI>();
        }
        CreateStep();
        if (null != TutoUI)
        {
            indicationText = TutoUI
                .transform.Find("indicationText")
                .GetComponent<TextMeshProUGUI>();
            indicationText.text = steps[currentIndexStep];
            isReadyText.text = "";
        }
    }

    public void CreateStep()
    {
        steps = new string[4]; // crée un tableau de 3 éléments
        steps[0] = "Droite - Gauche pour tourner";
        steps[1] = "Maintenir A ou Haut pour accélerer";
        steps[2] = "Maintienir B ou Bas pour Freiner/Reculer";
        steps[3] = "Appuie sur To. Dir Gauche / Droite pour Changer de peinture";
    }

    public void NextStep()
    {
        currentIndexStep += 1;
        if (currentIndexStep > 2)
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
        TutoUI.SetActive(show);
    }
}
