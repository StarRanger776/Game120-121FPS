using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private bool isInLava;

    private PlayerController player;

    private void Update()
    {
        if (isInLava && player != null)
        {
            player.canRegenHp = false;
        }
        else if (!isInLava && player != null)
        {
            player.canRegenHp = true;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        player = other.gameObject.GetComponent<PlayerController>();


        if (player != null)
        {
            isInLava = true;
            StartCoroutine(LavaDamage());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
            isInLava = false;
    }

    private IEnumerator LavaDamage()
    {
        if (player != null)
        {
            while (isInLava)
            {
                if (player.currentHp > 0)
                    player.currentHp -= 5;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
