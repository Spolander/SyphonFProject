using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaProjectile : MonoBehaviour {

    LineRenderer line;

    Vector3 targetPoint;
    Vector3 direction;
    Vector3 originPoint;
    float speed;
    float damage;
    bool damaged = false;

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
            line.SetPosition(0, originPoint + direction.normalized * speed * Time.deltaTime);

        }

        if (!damaged)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position, line.GetPosition(0), out hit, 1 << LayerMask.NameToLayer("Player")))
            {
                damaged = true;
                hit.collider.GetComponent<PlayerHealth>().takeDamage(damage, gameObject);
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, Time.deltaTime * speed);
        
	}

    public void Initialize(Vector3 targetPoint,Vector3 direction, float speed, float killDelay, float damage)
    {
        originPoint = transform.position;
        this.direction = direction;
        this.damage = damage;
        this.targetPoint = targetPoint;
        this.speed = speed;
        startingTime = Time.time;
        this.killDelay = killDelay;
        Destroy(gameObject, killDelay);
    }
}
