using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAwake : MonoBehaviour
{
    public string sceneToLoad;
    public PlayerController player;

    private void Awake()
    {
        if (player != null && sceneToLoad != null)
        {
            Camera mainCamera = Camera.main;
            player.transform.position = new Vector3(0, 0, 0);
            player.rotation.x = 0;
            player.rotation.y = -90;
            SceneManager.LoadScene(sceneToLoad);
        }
        else if (player != null && sceneToLoad == null)
        {
            Debug.LogError($"No scene to load in {this.gameObject.name}'s LoadSceneOnAwake script!");
        }
        else if (player == null && sceneToLoad != null)
        {
            Debug.LogError($"Player not set in {this.gameObject.name}'s LoadSceneOnAwake script!");
        }
        else
        {
            Debug.LogError($"Error in {this.gameObject.name}'s LoadSceneOnAwake script!");
        }
    }
}
