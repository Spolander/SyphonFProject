using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSpawner : MonoBehaviour {


    public GameObject prefab;

    public float interval = 0.5f;

    public float lastFireTime = 0;

    public float killDelay;
    public float speed;
    // Use this for initialization



    // Update is called once per frame
    void Update () {

        if (Time.time > lastFireTime + interval)
        {
            lastFireTime = Time.time;
            GameObject g = (GameObject)Instantiate(prefab, transform.position + Vector3.one * Random.Range(-0.2f, 0.2f), Quaternion.identity);
            g.GetComponent<MechaProjectile>().Initialize(transform.forward, speed, killDelay);
        }
		
	}
}
