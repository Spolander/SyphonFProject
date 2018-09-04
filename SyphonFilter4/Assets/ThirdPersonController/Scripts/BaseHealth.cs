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

    public void takeDamage(int amount) {
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death();
        }
    }

    void death()
    {
        Destroy(gameObject);
    }

    public void heal(int amount)
    {
        Health = Health + amount;
    }
}
