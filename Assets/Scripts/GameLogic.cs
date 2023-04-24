using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public bool enablePlayerMovementControls, enablePlayerCameraControls, enablePlayerGravity;
    public float timeScale = 1.0f;

    private void Update()
    {
        if (enablePlayerCameraControls)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!enablePlayerCameraControls)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
