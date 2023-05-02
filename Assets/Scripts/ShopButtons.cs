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
    private GameObject _playerUi;

    public GameObject startingCanvas;
    public GameObject shopCanvas;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        _mainCamera = Camera.main;
        _fpCam = FindObjectOfType<FirstPerson>().gameObject;
        _playerUi = FindObjectOfType<PlayerUI>().gameObject;
    }

    private void Start()
    {
        startingCanvas.SetActive(true);
        shopCanvas.SetActive(false);
    }

    public void NextLevel()
    {
        player.gameLogic.enablePlayerMovementControls = true;
        player.gameLogic.enablePlayerCameraControls = true;
        player.gameLogic.enablePlayerGravity = true;
        player.transform.position = spawnPosition;

        _playerUi.SetActive(true);
        _fpCam.SetActive(true);
        _mainCamera.gameObject.SetActive(true);
        player.gameObject.SetActive(true);

        player.rotation.x = 0;
        player.rotation.y = 0;

        SceneManager.LoadScene(sceneToLoad);
    }

    //Just made it barely functional. However you like to change this is up to you.
    public void ReturnToMainMenu() 
    {
        SceneManager.LoadScene("AltMainMenu");
    }

    public void EnterShop() 
    {
        startingCanvas.SetActive(false);
        shopCanvas.SetActive(true);
    }
}
