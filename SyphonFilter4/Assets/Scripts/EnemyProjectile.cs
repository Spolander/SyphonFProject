using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    //damage that projectile deals
    [SerializeField]
    public float damage;

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

    //backup to destroy projectiles if they dont hit anything
    //time when projectile was initialized
    float timeOfBirth;
    //projectile lifetime in seconds
    float maxLifeTime = 3;

    private void Start()
    {

    }

    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        if (Time.time > timeOfBirth + maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 targetLocation)
    {
        position = transform.position;
        this.targetLocation = targetLocation;
        transform.LookAt(targetLocation);
        timeOfBirth = Time.time;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (GetComponent<Deflectable>().isDeflected == false)
        {
            
            if (collision.GetComponent<BaseHealth>())
            {
                collision.GetComponent<BaseHealth>().takeDamage(damage, gameObject);
            }  
            else
                Debug.Log("collideWithWALL");
        }
        Destroy(gameObject);
        //Set hit to true to exit the coroutine loop
        hit = true;
    }
}
