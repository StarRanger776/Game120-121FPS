using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject optionsCanvas;
    public GameObject mainCanvas;

    private Settings curSettings;

    private void Awake()
    {
        curSettings = Settings.instance;
    }

    private void OnEnable()
    {
        optionsCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LoadFirstAfterMainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Options() 
    { 
        if(optionsCanvas != null) 
        {
            optionsCanvas.SetActive(true);
            mainCanvas.SetActive(false);
        }
        else 
        {
            Debug.Log("ERROR: No Options Menu Found");
        }
    }

    //Options Menu
    public void exitOptions() 
    {
        if (mainCanvas != null)
        {
            mainCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("ERROR: No Main Menu Found");
        }
    }
    public void Placeholder() { return; }
}

