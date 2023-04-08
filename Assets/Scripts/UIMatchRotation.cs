using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMatchRotation : MonoBehaviour
{
    private GameObject objectToRotateWith;

    private void Start()
    {
        objectToRotateWith = Camera.main.gameObject;
    }

    private void Update()
    {
        this.gameObject.transform.rotation = new Quaternion(objectToRotateWith.transform.rotation.x, objectToRotateWith.transform.rotation.y, objectToRotateWith.transform.rotation.z, objectToRotateWith.transform.rotation.w);
    }
}
