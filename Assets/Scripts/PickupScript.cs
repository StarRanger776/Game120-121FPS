using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    private ItemBase item;
    bool usePickupText = true;

    private void Start()
    {
        item = GetComponent<ItemBase>();

        if (item != null && item.pickupText != null)
        {
            if (usePickupText)
                item.pickupText.text = $"Press 'E' to pickup {item.name}";

            item.pickupText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) // make sure whatever this script is attached to has a trigger collider
    {
        // only the player will have the PlayerController script, so we can just test for that
        PlayerController player = other.GetComponent<PlayerController>();

        // make sure we are only interacting with the player
        if (player != null)
        {
            if (item.pickupText != null)
                item.pickupText.gameObject.SetActive(true);
            item.readyToBePickedUp = true;
            player.itemToPickup = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            item.readyToBePickedUp = false;
            if (item.pickupText != null)
                item.pickupText.gameObject.SetActive(false);
            if (player.itemToPickup == item)
                player.itemToPickup = null;
        }
    }
}
