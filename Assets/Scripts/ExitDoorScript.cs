using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorScript : MonoBehaviour
{
    public string sceneToLoad;
    public bool canUseExit = false;
    public List<Enemy> enemiesToKill = new List<Enemy>();
    public Vector3 spawnPoint = new Vector3(0, 0, 0);
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        for (int i = 0; i < enemiesToKill.Count; ++i)
        {
            if (enemiesToKill[i].currentHp <= 0)
            {
                enemiesToKill.RemoveAt(i);
            }
        }

        if (enemiesToKill.Count == 0)
        {
            canUseExit = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canUseExit)
        {
            player.transform.position = spawnPoint;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
