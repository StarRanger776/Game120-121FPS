using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class LevelEntryManager : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (player.canRegenFuel) // this will restart the coroutine that managers fuel regen, as it does not continue between levels
        {
            StartCoroutine(ResetFuelRegen());
        }

        if (player.canRegenHp) // same as fuelregen
        {
            StartCoroutine(ResetHealthRegen());
        }
    }

    private IEnumerator ResetFuelRegen()
    {
        player.canRegenFuel = false;
        yield return new WaitForEndOfFrame();
        player.isRegeningFuel = false;
        yield return new WaitForEndOfFrame();
        player.canRegenFuel = true;
        yield return new WaitForEndOfFrame();
        player.isRegeningFuel = true;
    }

    private IEnumerator ResetHealthRegen()
    {
        player.canRegenHp = false;
        yield return new WaitForEndOfFrame();
        player.isRegeningHp = false;
        yield return new WaitForEndOfFrame();
        player.canRegenHp = true;
        yield return new WaitForEndOfFrame();
        player.isRegeningHp = true;
    }
}
