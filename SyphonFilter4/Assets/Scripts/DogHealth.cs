using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogHealth : MonoBehaviour {

    public float playerDamage = 10;
    private float swordLenght = 5;

    private float health = 50;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
      //  Debug.Log(GameObject.Find("Goiruus").GetComponent<DogAI>().playerDistance + " " + swordLenght);
		if(GameObject.Find("Goiruus").GetComponent<DogAI>().playerDistance <= swordLenght)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("HIT");
            }
        }
	}

}
