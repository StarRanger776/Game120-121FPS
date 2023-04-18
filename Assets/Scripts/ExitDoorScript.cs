using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoorScript : MonoBehaviour
{
    public string sceneToLoad;
    public bool canUseExit = false;
    public List<Enemy> enemiesToKill = new List<Enemy>();

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
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
