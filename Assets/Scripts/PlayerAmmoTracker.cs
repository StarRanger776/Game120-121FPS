using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAmmoTracker : MonoBehaviour
{
    private PlayerController player;
    private TextMeshProUGUI ammoText;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        ammoText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (player.gameObject.activeInHierarchy && ammoText != null && !player.isReloading)
        {
            if (player.currentWeapon.type.ToUpper().Equals("PISTOL"))
                ammoText.text = $"{player.currentWeapon.loadedAmmo} / {player.currentWeapon.maxAmmoBeforeReload} - ({player.currentPistolAmmo})";
            else if (player.currentWeapon.type.ToUpper().Equals("RIFLE"))
                ammoText.text = $"{player.currentWeapon.loadedAmmo} / {player.currentWeapon.maxAmmoBeforeReload} - ({player.currentRifleAmmo})";
            else if (player.currentWeapon.type.ToUpper().Equals("LASER"))
                ammoText.text = $"{player.currentWeapon.loadedAmmo} / {player.currentWeapon.maxAmmoBeforeReload} - ({player.currentLaserAmmo})";
        }
        else if (player.gameObject.activeInHierarchy && ammoText != null && player.isReloading)
        {
            ammoText.text = $"Reloading...";
        }
    }
}
