using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneLoadScript : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null && sceneToLoad != null)
        {
            Camera mainCamera = Camera.main;
            player.transform.position = new Vector3(0, 0, 0);
            player.rotation.x = 0;
            player.rotation.y = -90;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
