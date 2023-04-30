using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiscButtons : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void ReturnToMainMenu()
    {
        if (player != null )
        {
            Destroy(player.gameObject);
            Destroy(Camera.main.gameObject);
            Destroy(FindObjectOfType<PlayerUI>().gameObject);
            Destroy(FindObjectOfType<FirstPerson>().gameObject);
            Destroy(player.gameLogic.gameObject);
            player = null;
        }

        // SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene("AltMenuMain");
    }

    public void ExitGame()
    {
        // EditorApplication.isPlaying = false; // comment out or delete before building, else the project will fail to build
        Application.Quit();
    }
}
