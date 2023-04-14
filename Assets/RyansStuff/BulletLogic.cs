using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    public float speed = 20f;
    public float lifeSpan = 10;
    public float startingLifeSpan = 10;
    public int bulletDamage;


    [Tooltip("units per second")]
    public float moveSpeed = 10f;



    // Start is called before the first frame update
    private void Start()
    {
        lifeSpan = startingLifeSpan;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 offset = transform.forward * (speed * Time.deltaTime);
        transform.position += offset;

        //offset = offset * (moveSpeed * Time.deltaTime);
        //saves cpu preformance
        // transform.position += offset;

        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0) Destroy(this.gameObject);

    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag.Equals("Player"))
        {
            collider.gameObject.GetComponent<PlayerController>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }
        else if (!collider.gameObject.tag.Equals("IgnoreBulletCollisions"))
        {
            Destroy(this.gameObject);
        }
    }
}