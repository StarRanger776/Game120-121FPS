using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    public bool firingLaser = false;
    public bool called = false;
    Vector3 sizeToScale = new Vector3(0.001f, 0, 0.001f);
    Vector3 rotation = new Vector3(0, 1, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(called == true)
        {
            StartCoroutine(FireLaser());
            called = false;
        }
        if(firingLaser == true)
        {
            gameObject.transform.localScale += sizeToScale;
            gameObject.transform.Rotate(rotation);
        }
    }

    public IEnumerator FireLaser()
    {
        firingLaser = true;
        yield return new WaitForSeconds(3);
        firingLaser = false;
    }
}
