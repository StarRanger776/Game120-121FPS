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

    [Header("Misc")]
    public LayerMask ShootRaycastIgnore;

    public void ShootRaycast()
    {
        if (isRaycast)
        {
            RaycastHit hit; //new raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //direct raycast from camera to mouse position

            if (Physics.Raycast(ray, out hit, 100, ~ShootRaycastIgnore))
            {
                Enemy enemyToDamage;

                Transform objectHit = hit.transform;

                if (objectHit.parent != null)
                {
                    enemyToDamage = objectHit.parent.GetComponent<Enemy>();
                }
                else
                {
                    enemyToDamage = objectHit.GetComponent<Enemy>();
                }

                Debug.Log(objectHit.name);

                if (enemyToDamage != null)
                {
                    enemyToDamage.TakeDamage(damage);
                }
            }
        }
    }

    public void ShootBullet()
    {
        
    }
}
