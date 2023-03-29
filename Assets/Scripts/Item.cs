using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string type; // available types: KEY, ITEM (add more as needed, dont add a WEAPON type here)
    public bool canBePickedUp;
}
