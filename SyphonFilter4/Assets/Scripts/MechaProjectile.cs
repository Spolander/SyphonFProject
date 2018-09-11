using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaProjectile : MonoBehaviour {

    LineRenderer line;

    Vector3 direction;
    float speed;

    float startingTime;
    private float killDelay;

    Vector3 startingPoint;
	// Use this for initialization
	void Start () {
        startingPoint = transform.position;
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
	}
	
	// Update is called once per frame
	void Update () {

        line.SetPosition(1, transform.position);

        if (Time.time > startingTime + killDelay / 3)
        {
            line.SetPosition(0, line.GetPosition(0) + direction.normalized * speed * Time.deltaTime);

        }

        transform.Translate(direction * Time.deltaTime * speed, Space.World);
	}

    public void Initialize(Vector3 dir, float speed, float killDelay)
    {
        direction = dir;
        this.speed = speed;
        startingTime = Time.time;
        this.killDelay = killDelay;
        Destroy(gameObject, killDelay);
    }
}
