using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Color color1;
    public Color color2;
    public void ActivateLaser()
    {
        meshRenderer.material.color = color2;
    }
}
