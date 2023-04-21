using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LaserGun : MonoBehaviour
{
    public bool firingLaser = false;
    public bool called = false;
    public Vector3 sizeToScale = new Vector3(0.001f, 0, 0.001f); // needs to be public so we can change it in the inspector
    Vector3 rotation = new Vector3(0, 1, 0);
    public List<LaserSwitch> switches = new List<LaserSwitch>();
    private int activatedCount = 0;
    public Vector3 maxSize = new Vector3(20, 100, 20);
    private AudioSource _fireSound;
    private AudioSource _ambientSound; // plays after initial fire noise
    public Enemy boss;


    private void Awake()
    {
        _fireSound = GetComponent<AudioSource>();
        _ambientSound = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Update()
    {
        if (called == true)
        {
            StartCoroutine(FireLaser());
            called = false;
        }
        if (firingLaser == true)
        {
            if (gameObject.transform.localScale.x < maxSize.x && gameObject.transform.localScale.y < maxSize.y && gameObject.transform.localScale.z < maxSize.z)
                gameObject.transform.localScale += sizeToScale;
            gameObject.transform.Rotate(rotation);
        }
    }

    private void FixedUpdate()
    {
        if (activatedCount == -32767)
        {
            if (!_fireSound.isPlaying && !_ambientSound.isPlaying)
                _ambientSound.Play();
        }
        else if (activatedCount >= switches.Count)
        {
            called = true;
            activatedCount = -32767;
        }
        else
        {
            activatedCount = 0;

            for (int i = 0; i < switches.Count; i++)
            {
                if (switches[i].activated)
                {
                    activatedCount += 1;
                }
            }
        }
    }

    public IEnumerator FireLaser()
    {
        _fireSound.Play();
        yield return new WaitForSeconds(2);
        firingLaser = true;
        yield return new WaitForSeconds(0.5f);
        boss.currentHp = 0;
        // Destroy(this.gameObject);
    }
}
