using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    public float speed = 20f;
    public float lifeSpan = 10;
    public float startingLifeSpan = 10;


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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this);
        }
    }
}