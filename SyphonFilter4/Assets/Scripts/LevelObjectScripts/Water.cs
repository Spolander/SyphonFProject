﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    [SerializeField]
    int damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<PlayerHealth>().takeDamage(damage, gameObject);
    }
}
