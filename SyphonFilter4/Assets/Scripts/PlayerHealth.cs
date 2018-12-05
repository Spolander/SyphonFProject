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

    [SerializeField]
    float movementSpeedSlowTime = 4;

    float originalMoveSpeed;
    float damageTaken;

    playerHealthBar hpBar;


    private void Start()
    {
        Health = Mathf.Clamp(Health, 1, maxHealth);
        hpBar = playerHealthBar.m_playerHealthBar;
        originalMoveSpeed = GetComponent<PlayerCharacterController>().moveSpeed;
    }

    private void Update()
    {
        //speeding up after taking damage
        if (Time.time < damageTaken+ movementSpeedSlowTime)
        {
            GetComponent<PlayerCharacterController>().moveSpeed = Mathf.MoveTowards(GetComponent<PlayerCharacterController>().moveSpeed, originalMoveSpeed, 0.05f);
        }
    }
    public override void takeDamage(float amount, GameObject caller)
    {
        if (Time.time < lastDamageTime + postHitInvincibility)
        {
            return;
        }
        lastDamageTime = Time.time;
        Health = Health - amount;

        //for slowing movement speed
        damageTaken = Time.time;
        //originalMoveSpeed = GetComponent<PlayerCharacterController>().moveSpeed;
        GetComponent<PlayerCharacterController>().moveSpeed = originalMoveSpeed / 4;

        if (hpBar != null)
            hpBar.UpdateHealthBar(maxHealth, Health);

        if (Health <= 0)
        {
            death(caller);
        }
    }

    public void heal(float amount, GameObject caller)
    {
        if (Health + amount < maxHealth)
        {
            Health = Health + amount;

        }
        else if (Health + amount >= maxHealth)
        {
            Health = maxHealth;
        }
        if (hpBar != null)
        {
            hpBar.UpdateHealthBar(maxHealth, Health);           //health bar update
        }
        Debug.Log(Health);   
    }


    //getters for checking values
    public float GetHealth()
    {
        return Health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
