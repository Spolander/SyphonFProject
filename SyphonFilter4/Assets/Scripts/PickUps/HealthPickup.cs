﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    [SerializeField]
    int HealAmount = 20;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right * Time.deltaTime*100);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //if players health is already at max, don't do anything
            if (other.GetComponent<PlayerHealth>().GetHealth() != other.GetComponent<PlayerHealth>().GetMaxHealth())
            {
                other.GetComponent<PlayerHealth>().heal(HealAmount, gameObject);
                Destroy(gameObject);
            }
        }
    }
}
