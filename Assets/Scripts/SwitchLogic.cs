using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLogic : MonoBehaviour
{
    public bool isActivated = false;
    private bool readyToActivate = false; // used for trigger testing

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.switchToActivate = this;
            readyToActivate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            player.switchToActivate = null;
            readyToActivate = false;
        }
    }
}
