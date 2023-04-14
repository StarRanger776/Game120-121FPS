using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject optionsCanvas;
    public GameObject mainCanvas;

    public float xSens;
    public float ySens;

    private Settings curSettings;

    private void Awake()
    {
        curSettings = Settings.instance;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LoadFirstAfterMainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //Options Menu
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

