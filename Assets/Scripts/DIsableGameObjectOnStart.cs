using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIsableGameObjectOnStart : MonoBehaviour
{
    private void LateUpdate()
    {
        this.gameObject.SetActive(false);
        Destroy(this);
    }
}
