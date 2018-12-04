using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    //damage that projectile deals
    [SerializeField]
    private float damage;

    //where projectile is headed
    Vector3 targetLocation;
    //where projectile is
    Vector3 position;

    //time that it takes for the projectile to arrive
    private float projectileTime;

    //speed of the projectile
    [SerializeField]
    public float projectileSpeed;

    bool hit = false;

    private void Start()
    {
        Initialize(PlayerCharacterController.player.GetComponent<BaseHealth>().centerPoint, projectileSpeed, 50);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        //position = Vector3.MoveTowards(position, targetLocation,projectileSpeed*Time.deltaTime );
        //transform.position = position;

        if (transform.position == targetLocation)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 targetLocation,float speed, float damage)
    {
        this.damage = damage;
        position = transform.position;
        this.targetLocation = targetLocation;
        projectileSpeed = speed;
        transform.LookAt(PlayerCharacterController.player.GetComponent<BaseHealth>().centerPoint);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GetComponent<Deflectable>().isDeflected == false)
        {
            Debug.Log("collide");
            if (collision.GetComponent<BaseHealth>())
            {
                collision.GetComponent<BaseHealth>().takeDamage(damage, gameObject);

            }
            //Set hit to true to exit the coroutine loop
            hit = true;
        }
        Destroy(gameObject);
    }
}
