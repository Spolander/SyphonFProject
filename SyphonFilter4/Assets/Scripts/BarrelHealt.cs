using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelHealt : BaseHealth {

    public override void takeDamage(float amount, GameObject caller)
    {
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death(caller);
        }
    }

    public override void death(GameObject caller)
    {

        if (caller.tag == "Player")
        {
            caller.GetComponent<playerCombat>().stopLockOn();
        }
        GetComponent<barrel>().Explode();
        Destroy(gameObject);
    }
}
