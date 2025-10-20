using TMPro; // nécessaire si tu utilises TextMeshPro
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuKartManager : MonoBehaviour
{
    public GameObject menuKart;

    // Lap Management
    public TMP_Text textLap; // Pour TextMeshPro
    private int nbLap = 1;
    public int maxLap = 8;

    // Buttons
    public Button lapPlus;
    public Button lapMinus;
    public Button playButton;
    public Button mainMenuButton;

    void Awake()
    {
        // Assign if null
        if (null == lapPlus)
        {
            lapPlus = GameObject.Find("LapPlus").GetComponent<Button>();
        }
        if (null == lapMinus)
        {
            lapMinus = GameObject.Find("LapMinus").GetComponent<Button>();
        }
        if (null == playButton)
        {
            playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        }
        if (null == mainMenuButton)
        {
            mainMenuButton = GameObject.Find("MainMenuButton").GetComponent<Button>();
        }

        if (null == textLap)
        {
            textLap = GameObject.Find("TextLap").GetComponent<TMP_Text>();
        }
        if (null == menuKart)
        {
            menuKart = GameObject.Find("MenuKart");
        }

        // Init All Buttons

        if (null != lapPlus)
        {
            lapPlus.onClick.AddListener(() => OnLapPlusClicked());
        }
        if (null != lapMinus)
        {
            lapMinus.onClick.AddListener(() => OnLapLessClicked());
        }
        if (null != playButton)
        {
            playButton.onClick.AddListener(() => OnPlayClicked());
        }
        if (null != mainMenuButton)
        {
            mainMenuButton.onClick.AddListener(() => OnMainMenuButton());
        }

        if (nbLap <= 1)
        {
            EnableInterractButton(lapMinus, false);
        }
        else if (nbLap >= maxLap)
        {
            EnableInterractButton(lapPlus, false);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Escape");
            menuKart.SetActive(true);
        }
    }

    // --- Functions to call when clicked on ---
    void OnLapPlusClicked()
    {
        IncrementValue();
        if (nbLap >= maxLap)
        {
            EnableInterractButton(lapPlus, false);
        }
        else
        {
            EnableInterractButton(lapMinus, true);
        }
        textLap.text = nbLap.ToString();
    }

    void OnLapLessClicked()
    {
        DecrementValue();
        if (nbLap <= 1)
        {
            EnableInterractButton(lapMinus, false);
        }
        else
        {
            EnableInterractButton(lapPlus, true);
        }
        textLap.text = nbLap.ToString();
    }

    void OnPlayClicked()
    {
        menuKart.SetActive(false);
    }

    void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Functions

    public void IncrementValue()
    {
        nbLap++;
    }

    public void DecrementValue()
    {
        nbLap--;
    }

    void EnableInterractButton(Button button, bool enable)
    {
        button.interactable = enable;
    }
}
