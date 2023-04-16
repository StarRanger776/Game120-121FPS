using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogicScript : MonoBehaviour
{
    public int damage;

     private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
            other.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(this);
    }
}
