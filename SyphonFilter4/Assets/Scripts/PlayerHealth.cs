using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth{

    //last time when damage was taken
    float lastDamageTime;

    //the time before you can take damage after previous takeDamage()
    float postHitInvincibility = 0.2f;
    public override void takeDamage(float amount, GameObject caller)
    {
        if (Time.time < lastDamageTime + postHitInvincibility)
        {
            return;
        }
        lastDamageTime = Time.time;
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death(caller);
        }
    }
}
