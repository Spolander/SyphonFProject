using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour {

    [SerializeField]
    private int Health = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void takeDamage(int amount, GameObject caller) {
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

    public virtual void heal(int amount)
    {
        Health = Health + amount;
    }
}
