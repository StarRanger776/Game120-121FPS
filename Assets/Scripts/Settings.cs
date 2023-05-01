using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings instance;
    //The idea being that you can just plug this in OnEnable or something, in the player Controller 
    public Vector2 savedSens;

    private float xSens;
    private float ySens;

    //Just Making it a singleton.
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
        savedSens.x = xSens;
        savedSens.y = ySens;
        
        //Wanted to localize it, for when Save/Load, that way Settings are saved after restarting game...
        //savedSens = PlayerController.mouseSensitivity;
    }

}
