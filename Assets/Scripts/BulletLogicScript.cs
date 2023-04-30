using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogicScript : MonoBehaviour
{
  public int damage;
  public float lifeSpan = 2;

  void FixedUpdate()
  {
    if(lifeSpan > 0)
        lifeSpan -= Time.deltaTime;
    else
        Destroy(this.gameObject);
  }

    private void OnCollisionEnter(Collision other)
    {
        Enemy enemy;
        if(other.gameObject.transform.parent != null)
        {
            enemy = other.gameObject.transform.parent.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
                print(this.gameObject);
                Destroy(this.gameObject);
            }
        }
        else if(other.gameObject.transform.parent == null)
        {
            enemy = other.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
                print(this.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
