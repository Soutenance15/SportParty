using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPauseMiniGameManager : MonoBehaviour
{
    // Menu Pause
    public GameObject menuPause;
    public GameObject UI;
    public Button buttonContinue;
    public Button buttonBackMenu;
    public Button buttonQuitGame;

    // Confirm Back
    public GameObject confirmBack;
    public Button buttonConfirmBack;
    public Button buttonCancelBack;

    // Confirm Quit
    public GameObject confirmQuit;
    public Button buttonConfirmQuit;
    public Button buttonCancelQuit;

    // Button

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (null == UI)
        {
            UI = GameObject.Find("UI");
        }
        // Pannel Menu Pause
        if (null == menuPause)
        {
            menuPause = GameObject.Find("MenuPause");
        }
        if (null != menuPause && null == buttonContinue)
        {
            buttonContinue = menuPause.transform.Find("ButtonContinue").GetComponent<Button>();
        }
        if (null != menuPause && null == buttonBackMenu)
        {
            buttonBackMenu = menuPause.transform.Find("ButtonBackMenu").GetComponent<Button>();
        }
        if (null != menuPause && null == buttonQuitGame)
        {
            buttonQuitGame = menuPause.transform.Find("ButtonQuitGame").GetComponent<Button>();
        }
        // Listener
        if (null != buttonContinue)
        {
            buttonContinue.onClick.AddListener(() => OnContinue());
        }
        if (null != buttonBackMenu)
        {
            buttonBackMenu.onClick.AddListener(() => OnBackMenu());
        }
        if (null != buttonQuitGame)
        {
            buttonQuitGame.onClick.AddListener(() => OnQuitGame());
        }

        // Pannel Confirm Back
        if (null != menuPause && null == confirmBack)
        {
            confirmBack = menuPause.transform.Find("ConfirmBack").gameObject;
        }
        if (null != confirmBack && null == buttonConfirmBack)
        {
            buttonConfirmBack = confirmBack
                .transform.Find("ButtonConfirmBack")
                .GetComponent<Button>();
        }
        if (null != confirmBack && null == buttonCancelBack)
        {
            buttonCancelBack = confirmBack
                .transform.Find("ButtonCancelBack")
                .GetComponent<Button>();
        }
        // Listener
        if (null != buttonConfirmBack)
        {
            buttonConfirmBack.onClick.AddListener(() => OnConfirmBack());
        }
        if (null != buttonCancelBack)
        {
            buttonCancelBack.onClick.AddListener(() => OnCancelBack());
        }

        // Pannel Confirm Quit
        if (null != menuPause && null == confirmQuit)
        {
            confirmQuit = menuPause.transform.Find("ConfirmQuit").gameObject;
        }
        if (null != confirmQuit && null == buttonConfirmQuit)
        {
            buttonConfirmQuit = confirmQuit
                .transform.Find("ButtonConfirmQuit")
                .GetComponent<Button>();
        }
        if (null != confirmQuit && null == buttonCancelQuit)
        {
            buttonCancelQuit = confirmQuit
                .transform.Find("ButtonCancelQuit")
                .GetComponent<Button>();
        }
        // Listener
        if (null != buttonConfirmQuit)
        {
            buttonConfirmQuit.onClick.AddListener(() => OnConfirmQuit());
        }
        if (null != buttonCancelQuit)
        {
            buttonCancelQuit.onClick.AddListener(() => OnCancelQuit());
        }

        // Deactvate confirm panels
        // if (null != confirmBack)
        // {
        //     confirmBack.SetActive(false);
        // }
        // if (null != confirmQuit)
        // {
        //     confirmQuit.SetActive(false);
        // }
        if (null != menuPause)
        {
            menuPause.SetActive(false);
        }
    }

    public void ShowMenuPause(bool show)
    {
        if (show)
        {
            menuPause.SetActive(true);
            confirmBack.SetActive(false);
            confirmQuit.SetActive(false);
        }
        else
        {
            menuPause.SetActive(false);
        }
    }

    // CallBack funtion
    // Menu Pause
    void OnContinue()
    {
        Debug.Log("Continue");
    }

    void OnBackMenu()
    {
        confirmBack.SetActive(true);
        Debug.Log("Back Menu");
    }

    void OnQuitGame()
    {
        confirmQuit.SetActive(true);
        Debug.Log("Quit Game");
    }

    // Confirm Back
    void OnConfirmBack()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Confirm Back");
    }

    void OnCancelBack()
    {
        confirmBack.SetActive(false);
        Debug.Log("Cancel Back");
    }

    // Confirm Quit
    void OnConfirmQuit()
    {
        Debug.Log("Quitter le jeu...");
        Application.Quit();
    }

    void OnCancelQuit()
    {
        confirmQuit.SetActive(false);
        Debug.Log("Cancel Quit");
    }
}
