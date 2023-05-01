using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObjectOnStart : MonoBehaviour
{
    private void LateUpdate()
    {
        this.gameObject.SetActive(false);
        Destroy(this);
    }
}
