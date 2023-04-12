using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInvinc : MonoBehaviour
{
    private bool invincibleEnabled = false;
    [SerializeField]
    private float invincCooldown = 3.0f;
    private int speed = 5;
    void Update()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(direction * speed * Time.deltaTime);

    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (invincibleEnabled == false)
            {
                Destroy(gameObject);
            }
        }
    }
    public void InvincEnabled()
    {
        invincibleEnabled = true;
        StartCoroutine(InvincDisableRoutine());
    }
    IEnumerator InvincDisableRoutine()
    {
        yield return new WaitForSeconds(invincCooldown);
        invincibleEnabled = false;
    }

}

