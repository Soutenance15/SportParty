using TMPro;
using UnityEngine;

public class NextStepManager : MonoBehaviour
{
    GameObject TutoUI;
    TextMeshProUGUI indicationText;

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
        }
        CreateStep();
    }

    public void CreateStep()
    {
        steps = new string[3]; // crée un tableau de 3 éléments
        steps[0] = "Maintiens A/Fl.Haut pour accélerer";
        steps[1] = "Maintiens B/Fl.Bas pour Freiner/Reculer";
        steps[2] = "Appuie sur To. Dir Gauche / Droite pour Changer de peinture";
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
