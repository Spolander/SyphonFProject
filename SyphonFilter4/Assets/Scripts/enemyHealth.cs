using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : BaseHealth {

    public override void takeDamage(float amount, GameObject caller)
    {
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death(caller);
        }
        else
        {
            GetComponent<BaseAI>().LastAggroTime = Time.time;
        }
    }
}
