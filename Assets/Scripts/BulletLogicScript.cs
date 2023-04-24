using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogicScript : MonoBehaviour
{
  public int damage;
  public float lifeSpan = 3;

  void FixedUpdate()
  {
    if(lifeSpan > 0)
        lifeSpan -= Time.deltaTime;
    else
        Destroy(this.gameObject);
  }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            print(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
