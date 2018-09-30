using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidHazard : MonoBehaviour {

    [SerializeField]
    int damage;

    float previous;
    float delay = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        SoundEngine.instance.PlaySound("splash", gameObject.transform.position, gameObject.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<PlayerHealth>().takeDamage(damage, gameObject);
        if (Time.time > previous +delay)
        {
            SoundEngine.instance.PlaySound("waterWalk", gameObject.transform.position, gameObject.transform);
            previous = Time.time;
        }

    }
}
