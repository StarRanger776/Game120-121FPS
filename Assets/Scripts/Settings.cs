using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;

    public Vector2 savedSens;

    private void Awake()
    {
        if (Settings.instance == null)
        {
            Settings.instance = this;
        }
        else if (Settings.instance != this) 
        { 
            Destroy(this);
        }
    }

    private void Start()
    {
        //Wanted to localize it, for when Save/Load, that way Settings are saved after restarting game...
        //savedSens = PlayerController.mouseSensitivity;
    }

}
