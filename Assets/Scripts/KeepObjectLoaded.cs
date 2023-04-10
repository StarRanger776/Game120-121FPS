using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepObjectLoaded : MonoBehaviour
{
    private GameObject objectToKeepLoaded;

    private void Awake()
    {
        objectToKeepLoaded = this.gameObject;

        DontDestroyOnLoad(objectToKeepLoaded);
    }
}
