using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : ItemBase
{
    [Header("Weapon")]
    public int damage;
    public string damageType; // if we want different damage types such as PHYSICAL or MAGICAL or elemental like FIRE etc...
    public float attackRate; // should be attacks per second
    public float projSpeed; // speed of fired projectile
    public bool isRanged;
    public bool isTwoHanded;
    public bool isRaycast;

    public void ShootRaycast()
    {
        if (isRaycast)
        {
            RaycastHit hit; //new raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //direct raycast from camera to mouse position

            if (Physics.Raycast(ray, out hit, 100))
            {
                Transform objectHit = hit.transform;

                Debug.Log(objectHit.name);
            }
        }
    }

    public void ShootBullet()
    {
        
    }
}
