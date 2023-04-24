using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int currentHp;
    public uint maxHp;
    public Slider healthSlider;
    private PlayerController player;
    public int moneyOnKill;

    private void Awake()
    {
        if (currentHp <= 0)
            currentHp = (int)maxHp;

        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
            if (healthSlider != null)
                healthSlider.gameObject.SetActive(false);
        }
        else
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHp;
                healthSlider.maxValue = maxHp;
            }

            if (currentHp > (int)maxHp)
            {
                currentHp = (int)maxHp;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        player.currentMoney += moneyOnKill;
        player.moneyEarned += moneyOnKill;
    }
}
