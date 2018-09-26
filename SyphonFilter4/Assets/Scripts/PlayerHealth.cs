using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth{


    [SerializeField]
    private float maxHealth = 100;

   
    //last time when damage was taken
    float lastDamageTime;

    //the time before you can take damage after previous takeDamage()
    float postHitInvincibility = 0.2f;

    playerHealthBar hpBar;


    private void Start()
    {
        Health = Mathf.Clamp(Health, 1, maxHealth);
        hpBar = playerHealthBar.m_playerHealthBar;


    }
    public override void takeDamage(float amount, GameObject caller)
    {
        if (Time.time < lastDamageTime + postHitInvincibility)
        {
            return;
        }
        lastDamageTime = Time.time;
        Health = Health - amount;

        if (hpBar != null)
            hpBar.UpdateHealthBar(maxHealth, Health);

        if (Health <= 0)
        {
            death(caller);
        }
    }
}
