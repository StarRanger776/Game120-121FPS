using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaserSwitch : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Color color1;
    public Color color2;
    public TextMeshPro activateText;
    public bool activated = false;
    public bool withinBounds = false;
    public LaserGun laserGun;
    private AudioSource _activateSound;

    private void Awake()
    {
        _activateSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && activated == false && withinBounds == true)
        {
            ActivateSwitch();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            activateText.gameObject.SetActive(true);
            withinBounds = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            activateText.gameObject.SetActive(false);
            withinBounds = false;
        }
    }

    public void ActivateSwitch()
    {
        // laserGun.called = true; // need to test in the laserGun script to make sure each switch is activated
        activated = true;
        activateText.text = "Activated";
        meshRenderer.material.color = color2;
        _activateSound.Play();
    }
}
