using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour {

    [SerializeField]
    private float Health = 100;


    public virtual void takeDamage(float amount, GameObject caller) {
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death(caller);
        }
    }

    public virtual void death(GameObject caller)
        {
            
        if (caller.tag == "Player")
        {
            caller.GetComponent<playerCombat>().stopLockOn();
        }
        Destroy(gameObject);
    }

    public virtual void heal(float amount)
    {
        Health = Health + amount;
    }
}
