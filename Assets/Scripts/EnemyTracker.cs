using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private ExitDoorScript exitDoor;
    private TextMeshProUGUI trackerText;

    private void Awake()
    {
        exitDoor = FindObjectOfType<ExitDoorScript>();
        trackerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (exitDoor != null && trackerText != null && exitDoor.enemiesToKill.Count > 0)
        {
            trackerText.text = $"Kill all enemies and find the exit\nRemaining Enemies: {exitDoor.enemiesToKill.Count}";
        }
        else if (exitDoor != null && trackerText != null)
        {
            trackerText.text = $"Find the exit to proceed to the next level";
        }
    }
}
