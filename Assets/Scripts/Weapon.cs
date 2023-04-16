using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : ItemBase
{
    [Header("Weapon")]
    public int damage;
    public string damageType; // if we want different damage types such as PHYSICAL or MAGICAL or elemental like FIRE etc...
    public float attackRate; // should be attacks per second
    public float projSpeed; // speed of fired projectile
    public float spread;
    public bool isRanged;
    public bool isTwoHanded;
    public bool isRaycast;

    [Header("Misc")]
    public LayerMask ShootRaycastIgnore;
<<<<<<< Updated upstream
    public Transform attackPoint;
=======
    private Enemy _enemyToDamage;
    public GameObject weaponToActivateOnPlayer;

    public Transform attackPoint;
    [HideInInspector]
    public Weapon weaponToUseOnPlayer;
    public GameObject bullet;
    private Transform _shootPoint;
    private AudioSource _shootSound;
>>>>>>> Stashed changes

    public void ShootRaycast()
    {
<<<<<<< Updated upstream
        if (isRaycast)
=======
        if (weaponToActivateOnPlayer != null)
            weaponToUseOnPlayer = weaponToActivateOnPlayer.GetComponent<Weapon>();

        _shootPoint = this.transform.Find("Shoot Point");

        _shootSound = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        if (isRanged && readyToShoot)
        {
            if (isRaycast)
            {
                StartCoroutine(ShootRaycast());
            }
            else
            {
                StartCoroutine(ShootBullet());
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
>>>>>>> Stashed changes
        {
            RaycastHit hit; //new raycast
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //direct raycast from camera to mouse position

            if (Physics.Raycast(ray, out hit, 100, ~ShootRaycastIgnore))
            {
                Transform objectHit = hit.transform;

                Debug.Log(objectHit.name);

                if (objectHit.GetComponent<Enemy>())
                {
                    objectHit.GetComponent<Enemy>().TakeDamage(damage);
                }
            }
        }
    }

<<<<<<< Updated upstream

    public void ShootBullet()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(bullet);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit hit; 

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3 (x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;   
        Vector3 pos = Camera.main.transform.TransformPoint(Vector3.forward * 1);
        
        Rigidbody bulletRB = currentBullet.AddComponent(typeof(Rigidbody)) as Rigidbody;
        SphereCollider sc = currentBullet.AddComponent(typeof(SphereCollider)) as SphereCollider;
        BulletLogicScript BLScript = currentBullet.AddComponent<BulletLogicScript>();
        BLScript.damage = damage;
        sc.radius += 1f;
        sc.isTrigger = true;
        bulletRB.AddForce(directionWithSpread.normalized * 25, ForceMode.Impulse);
        //bulletRB.AddForce(Camera.main.transform.up * 1, ForceMode.Impulse); add camera recoil/shake?
=======
    private IEnumerator ShootBullet()
    {
        if (loadedAmmo > 0 && readyToShoot)
            {
                readyToShoot = false;
                loadedAmmo -= 1;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));
                RaycastHit hit;
                Vector3 targetPoint;
                if(Physics.Raycast(ray, out hit))
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

                
                //bulletRB.AddForce(Camera.main.transform.up * 1, ForceMode.Impulse); add camera recoil/shake?
        }
        yield return new WaitForSeconds(attackDelay);
        readyToShoot = true;
>>>>>>> Stashed changes
    }
}
