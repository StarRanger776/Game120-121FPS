using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoVisualizationScript : MonoBehaviour
{
    public Weapon weaponAmmoToTrack;
    private TextMeshPro _ammoText;

    private void Start()
    {
        _ammoText = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (weaponAmmoToTrack != null)
        {
            _ammoText.text = weaponAmmoToTrack.loadedAmmo.ToString();
        }
    }
}
