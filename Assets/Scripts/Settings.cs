using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    //Eventually when implementing the Save/Load thought it would be best if all information could just be found from here.
    public static Settings instance;
    public Vector2 savedSens;

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
        //Wanted to localize it, for when Save/Load, that way Settings are saved after restarting game...
        //savedSens = PlayerController.mouseSensitivity;
    }

}
