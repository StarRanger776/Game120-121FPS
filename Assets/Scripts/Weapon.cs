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
    public bool isChargable;
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
    private AudioSource _shootChargedSound;
    private AudioSource _shootUnchargedSound;

    private AudioSource _shootWhiffSound;
    private PlayerController _player;
    public Transform attackPoint;
    private Coroutine chargeShot;

    private void Start()
    {
        if (weaponToActivateOnPlayer != null)
            weaponToUseOnPlayer = weaponToActivateOnPlayer.GetComponent<Weapon>();

        _shootPoint = this.transform.Find("Shoot Point");

        _shootSound = GetComponent<AudioSource>();

        AudioSource[] shootSounds;

        shootSounds = GetComponents<AudioSource>();
        if(isChargable)
        {
            _shootWhiffSound = shootSounds[1];
            _shootUnchargedSound = shootSounds[2];
            _shootChargedSound = shootSounds[3];
        }   
        _player = FindObjectOfType<PlayerController>();
    }

    public void Shoot()
    {
        if (isRanged)
        {
            if (readyToShoot && isRaycast && !isChargable)
            {
                StartCoroutine(ShootRaycast());
            }
            else if(readyToShoot && !isRaycast && !isChargable)
            {
                StartCoroutine(ShootBullet());
            }
            else if (readyToShoot && !isRaycast && isChargable)
            {
                if (chargeShot == null)
                {
                    chargeShot = StartCoroutine(ShootChargedBullet());
                }
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

    private IEnumerator ShootBullet()
    {
        if (_shootSound != null)
                _shootSound.Play();
        print("whoops");
        if (loadedAmmo > 0 && readyToShoot)
        {
            readyToShoot = false;
            loadedAmmo -= 1;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithoutSpread.normalized;
            Vector3 pos = Camera.main.transform.TransformPoint(Vector3.forward * 1);

            Rigidbody bulletRB = currentBullet.AddComponent(typeof(Rigidbody)) as Rigidbody;
            SphereCollider sc = currentBullet.AddComponent(typeof(SphereCollider)) as SphereCollider;
            BulletLogicScript BLScript = currentBullet.AddComponent<BulletLogicScript>();
            BLScript.damage = damage;
            sc.radius += 1f;
            sc.isTrigger = true;
            bulletRB.AddForce(directionWithoutSpread.normalized * 25, ForceMode.Impulse);
        }

        //bulletRB.AddForce(Camera.main.transform.up * 1, ForceMode.Impulse); add camera recoil/shake?
        yield return new WaitForSeconds(attackDelay);
        readyToShoot = true;
    }
    private IEnumerator ShootChargedBullet()
    { 
        if (_shootSound != null)
                _shootSound.Play();
        float timeElapsed = 0;
        print("CHARGING...");
        while(Input.GetMouseButton(0))
        {
            yield return new WaitForSeconds(.01f);
            timeElapsed += Time.deltaTime;              
        }
        if(timeElapsed < 1.8)
        {
            _shootSound.Stop();
            _shootWhiffSound.Play();
            print("Zzzt...(Did not hold long enough)");
        }
        else if(timeElapsed < 3)
        {
            _shootUnchargedSound.Play();
            print(timeElapsed);
            readyToShoot = false;
            loadedAmmo -= 1;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithoutSpread.normalized;
            Vector3 pos = Camera.main.transform.TransformPoint(Vector3.forward * 1);

            Rigidbody bulletRB = currentBullet.AddComponent(typeof(Rigidbody)) as Rigidbody;
            SphereCollider sc = currentBullet.AddComponent(typeof(SphereCollider)) as SphereCollider;
            BulletLogicScript BLScript = currentBullet.AddComponent<BulletLogicScript>();
            BLScript.damage = damage;
            sc.radius += 1f;
            sc.isTrigger = true;
            bulletRB.AddForce(directionWithoutSpread.normalized * 25, ForceMode.Impulse);
        }
        else
        {   
            _shootChargedSound.Play();
            print(_shootChargedSound);
            print(timeElapsed);
            readyToShoot = false;
            loadedAmmo -= 1;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);
            Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithoutSpread.normalized;
            Vector3 pos = Camera.main.transform.TransformPoint(Vector3.forward * 1);
            Rigidbody bulletRB = currentBullet.AddComponent(typeof(Rigidbody)) as Rigidbody;
            bulletRB.useGravity = false;
            SphereCollider sc = currentBullet.AddComponent(typeof(SphereCollider)) as SphereCollider;
            BulletLogicScript BLScript = currentBullet.AddComponent<BulletLogicScript>();
            BLScript.damage = damage * 3;
            currentBullet.transform.localScale *= 3;
            sc.radius += 3f;
            sc.isTrigger = true;
            bulletRB.AddForce(directionWithoutSpread.normalized * 50, ForceMode.Impulse);

        }
        yield return new WaitForSeconds(attackDelay);
        readyToShoot = true;
        chargeShot = null;

    }
}