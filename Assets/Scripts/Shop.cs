using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private PlayerController player;
    private Camera _mainCamera;
    private GameObject _fpCam;
    private GameObject _playerUi;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        _mainCamera = Camera.main;
        _fpCam = FindObjectOfType<FirstPerson>().gameObject;
        _playerUi = FindObjectOfType<PlayerUI>().gameObject;

        player.gameLogic.enablePlayerCameraControls = false;
        player.gameLogic.enablePlayerGravity = false;
        player.gameLogic.enablePlayerMovementControls = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Start()
    {
        player.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(false);
        _fpCam.SetActive(false);
        _playerUi.SetActive(false);
    }
}
