using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [Header("Item")]
    public new string name;
    public string type; // available types: KEY, ITEM, WEAPON
    public bool canBePickedUp; // should the object be able to be picked up
    public TextMeshPro pickupText;
    //[HideInInspector]
    public bool readyToBePickedUp; // only set to true if player is in range and this item can be picked up
}
