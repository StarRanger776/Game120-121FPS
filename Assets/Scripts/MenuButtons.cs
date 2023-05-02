using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuButtons : MonoBehaviour
{
    public List<GameObject> listToEnable = new List<GameObject>();
    public bool overrideDefaultSceneLoad = false; // only for dev environment, gameLogic settings do not get changed by 
    public string sceneToLoad;
    
    public Slider mouseSensSlider;
    public TMP_Text sliderText;
   
    private PlayerController player;

    public GameObject mainCanvas;
    public GameObject optionsCanvas;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (mouseSensSlider != null)
        {
            mouseSensSlider.maxValue = 100;
            mouseSensSlider.minValue = 0;

            if (player.mouseSensitivity.x == 0)
                mouseSensSlider.value = 100;
        }
    }

    private void Start()
    {
        //Just making sure that options is not active.
        mainCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
    }

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

    //Options

    //Dunno if this is quite the most efficient way to do so, however I think a little bit can be given on the title screen.
    public void OptionsButton()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    private void Update()
    {
        //There might be a better place to put it, but for now I am stashing it in here, because that makes the most sense to me.
        if (sliderText != null)
        {
            float formattedValue = mouseSensSlider.value / 10f;
            sliderText.text = "Sensitivity:" + formattedValue.ToString("0.0");
        }
    }

    public void OptionsBack() 
    {
        mainCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
    }

}
