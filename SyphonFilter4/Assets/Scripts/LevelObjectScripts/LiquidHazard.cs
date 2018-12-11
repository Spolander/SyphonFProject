using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidHazard : MonoBehaviour {

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
        SoundEngine.instance.PlaySound("splash", gameObject.transform.position, gameObject.transform);
        other.GetComponent<PlayerCharacterController>().IsInWater = true;
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PlayerCharacterController>().IsInWater = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerCharacterController>())
        {
            other.GetComponent<PlayerHealth>().takeDamage(damage, gameObject);
        }
        
    }
}
