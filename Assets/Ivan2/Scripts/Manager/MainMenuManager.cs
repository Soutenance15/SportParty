using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // MiniGame Type
    private enum MiniGameType
    {
        None,
        Kart,
        Foot,
    }

    private MiniGameType selectedMiniGame = MiniGameType.None;

    // Buttons
    public Button kartButton;
    public Button footButton;
    public Button playButton;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    private Button currentSelectedButton; // selected button

    void Awake()
    {
        // Assign if null
        if (null == kartButton)
        {
            kartButton = GameObject.Find("KartButton").GetComponent<Button>();
        }
        if (null == footButton)
        {
            footButton = GameObject.Find("FootButton").GetComponent<Button>();
        }
        if (null == playButton)
        {
            playButton = GameObject.Find("PlayButton").GetComponent<Button>();
        }

        // Init All Buttons

        if (null != kartButton)
        {
            kartButton.onClick.AddListener(() => OnKartClicked());
        }
        if (null != footButton)
        {
            footButton.onClick.AddListener(() => OnFootClicked());
        }
        if (null != playButton)
        {
            playButton.onClick.AddListener(() => OnPlayClicked());
            EnableInterractButton(playButton, false);
        }
    }

    // --- Functions to call when clicked on ---

    void OnKartClicked()
    {
        Debug.Log("Kart");
        EnableInterractButton(playButton, true);
        selectedMiniGame = MiniGameType.Kart;
        SelectButton(kartButton);
    }

    void OnFootClicked()
    {
        Debug.Log("Foot");
        EnableInterractButton(playButton, true);
        selectedMiniGame = MiniGameType.Foot;
        SelectButton(footButton);
    }

    void OnPlayClicked()
    {
        switch (selectedMiniGame)
        {
            case MiniGameType.Kart:
                SceneManager.LoadScene("KartScene");
                break;
            case MiniGameType.Foot:
                SceneManager.LoadScene("FootScene");
                break;
        }
    }

    void SelectButton(Button button)
    {
        // reset current color selected button
        if (currentSelectedButton != null)
            currentSelectedButton.image.color = normalColor;

        // change current selected button and his color
        currentSelectedButton = button;
        currentSelectedButton.image.color = selectedColor;
    }

    //--- Utils Functions ---

    void EnableInterractButton(Button button, bool enable)
    {
        button.interactable = enable;
    }
}
