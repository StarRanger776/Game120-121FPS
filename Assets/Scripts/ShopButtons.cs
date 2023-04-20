using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButtons : MonoBehaviour
{
    private PlayerController player;
    public string sceneToLoad;
    private Camera _mainCamera;
    public Vector3 spawnPosition;
    private GameObject _fpCam;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        _mainCamera = Camera.main;
        _fpCam = FindObjectOfType<FirstPerson>().gameObject;
    }

    public void NextLevel()
    {
        player.gameLogic.enablePlayerMovementControls = true;
        player.gameLogic.enablePlayerCameraControls = true;
        player.gameLogic.enablePlayerGravity = true;
        player.transform.position = spawnPosition;

        _fpCam.SetActive(true);
        _mainCamera.gameObject.SetActive(true);
        player.gameObject.SetActive(true);

        SceneManager.LoadScene(sceneToLoad);
    }
}
