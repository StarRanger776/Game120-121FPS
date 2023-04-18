using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnAwake : MonoBehaviour
{
    public string sceneToLoad;
    private PlayerController player;
    public Vector3 spawnPoint;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (player != null && sceneToLoad != null)
        {
            Camera mainCamera = Camera.main;
            player.transform.position = new Vector3(0, 0, 0);
            player.rotation.x = 0;
            player.rotation.y = -90;
            SceneManager.LoadScene(sceneToLoad);
            player.gameLogic.enablePlayerCameraControls = true;
            player.gameLogic.enablePlayerMovementControls = true;
            player.gameLogic.enablePlayerGravity = true;
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
