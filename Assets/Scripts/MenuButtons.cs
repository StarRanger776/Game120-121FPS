using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject optionsCanvas;
    public GameObject mainCanvas;

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
    public void Placeholder() { return; }
}
