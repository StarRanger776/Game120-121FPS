using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : ItemBase
{
    [Header("Weapon")]
    public int damage;
    public string damageType; // if we want different damage types such as PHYSICAL or MAGICAL or elemental like FIRE etc...
    public float attackDelay; // how long between attacks/shots
    public float projSpeed; // speed of fired projectile
    public bool isRanged;
    public bool isTwoHanded;
    public bool isRaycast;
    public int loadedAmmo; // ammo in "clip"
    public uint maxAmmoBeforeReload; // max ammo in "clip"
    public bool readyToShoot = true;
    public float reloadTime = 1.0f; // time to reload in seconds
    public float lowRecoilAmount; // low is the lowest possible recoil amount, high is the highest possible recoil amount
    public float highRecoilAmount; // will need to tweak it to get your desired recoil for each weapon

    [Header("Misc")]
    public LayerMask ShootRaycastIgnore;
    private Enemy _enemyToDamage;
    public GameObject weaponToActivateOnPlayer;
    [HideInInspector]
    public Weapon weaponToUseOnPlayer;
    public GameObject bullet;
    private Transform _shootPoint;
    private AudioSource _shootSound;
    private PlayerController _player;

    private void Start()
    {
        if (weaponToActivateOnPlayer != null)
            weaponToUseOnPlayer = weaponToActivateOnPlayer.GetComponent<Weapon>();

        _shootPoint = this.transform.Find("Shoot Point");

        _shootSound = GetComponent<AudioSource>();

        _player = FindObjectOfType<PlayerController>();
    }

    public void Shoot()
    {
        if (isRanged)
        {
            if (isRaycast && readyToShoot)
            {
                StartCoroutine(ShootRaycast());
            }
            else
            {
                ShootBullet();
            }
        }
        else
        {
            Debug.Log("Insert melee attack here!");
        }
    }

    private IEnumerator ShootRaycast()
    {
        if (loadedAmmo > 0 && readyToShoot)
        {
            RaycastHit hit; //new raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //direct raycast from camera to mouse position
            readyToShoot = false;
            loadedAmmo -= 1;
            _player.rotation.x -= UnityEngine.Random.Range(lowRecoilAmount, highRecoilAmount);
            if (_shootSound != null)
                _shootSound.Play();

            if (Physics.Raycast(ray, out hit, 100, ~ShootRaycastIgnore))
            {
                Transform objectHit = hit.transform;

                if (objectHit.parent != null)
                {
                    _enemyToDamage = objectHit.parent.GetComponent<Enemy>();
                }
                else
                {
                    _enemyToDamage = objectHit.GetComponent<Enemy>();
                }

                Debug.Log(objectHit.name);

                if (_enemyToDamage != null)
                {
                    _enemyToDamage.TakeDamage(damage);
                }
            }

            // play shooting sound

        }
        yield return new WaitForSeconds(attackDelay);

        readyToShoot = true;
    }

    private void ShootBullet()
    {
        if (loadedAmmo > 0)
        {
            //GameObject currentBullet = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        }
    }
}
