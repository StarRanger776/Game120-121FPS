using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public List<GameObject> listToEnable = new List<GameObject>();
    public bool overrideDefaultSceneLoad = false; // only for dev environment, gameLogic settings do not get changed by 
    public string sceneToLoad;
    public Slider mouseSensSlider;
    private PlayerController player;

    //Assume these change the formatting in the Editor, however Unity will not allow me to run the game without these being resolved.

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

//<<<<<<< HEAD
        if (mouseSensSlider != null)
        {
            mouseSensSlider.maxValue = 100;
            mouseSensSlider.minValue = 0;

            if (player.mouseSensitivity.x == 0)
                mouseSensSlider.value = 100;
        }
    }

//=======
//>>>>>>> parent of 09c5c22 (Shop and Music)
    public void StartGame()
    {
        if (!overrideDefaultSceneLoad)
            SceneManager.LoadScene("LoadFirstAfterMainMenu");
        else
            SceneManager.LoadScene(sceneToLoad);

        if (listToEnable != null)
        {
            for (int i = 0; i < listToEnable.Count; i++)
            {
                listToEnable[i].SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("Nothing to enable in list.");
        }
    }

    public void ExitGame()
    {
        // EditorApplication.isPlaying = false; // comment out or delete before building, else the project will fail to build
        Application.Quit();
    }
}
