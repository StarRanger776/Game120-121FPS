using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public List<GameObject> listToEnable = new List<GameObject>();
    public bool overrideDefaultSceneLoad = false; // only for dev environment, gameLogic settings do not get changed by 
    public string sceneToLoad;

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

    }
}
