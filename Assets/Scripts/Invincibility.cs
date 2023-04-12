using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class Invincibility : MonoBehaviour

{

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "PowerUp")
        {
            this.GetComponent<PlayerHealth>().enabled = false;
        }
        Invoke("InvicOn", 10);
    }
    
}
