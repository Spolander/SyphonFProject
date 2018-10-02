using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyThrowable : MonoBehaviour {

    private float projectileTime;

    [SerializeField]
    private float heightMultiplier = 1;
    Vector3 targetLocation;
    Vector3 startingPoint;

    [SerializeField]
    private AnimationCurve flightCurve;

    [SerializeField]
    private GameObject explosionEffect;


    [SerializeField]
    private float hitDetectionRadius = 1f;

    [SerializeField]
    private float explosionRadius = 4;

    [SerializeField]
    private LayerMask collisionLayers;

    private float damage;

    bool hit = false;

    Transform spriteTransform;

    Camera mainCam;
    public void Initialize(Vector3 targetLocation, float time, float damage)
    {
        mainCam = Camera.main;
        spriteTransform = transform.GetChild(0);
        this.damage = damage;
        startingPoint = transform.position;
        this.targetLocation = targetLocation;
        projectileTime = time;
        StartCoroutine(moveAnimation());
    }

    IEnumerator moveAnimation()
    {
        float lerp = 0;
       
        while (lerp <= 1 && !hit)
        {
            spriteTransform.rotation = Quaternion.LookRotation(mainCam.transform.position - transform.position);
            Vector3 pos = Vector3.Lerp(startingPoint, targetLocation, lerp);
            pos.y += flightCurve.Evaluate(lerp) * heightMultiplier;

            lerp += Time.deltaTime / projectileTime;
            transform.position = pos;
            HitDetection();
            yield return null;
        }

        Destroy(gameObject);
        SoundEngine.instance.PlaySound("grenade", transform.position, null);
        GameObject particleSys = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);
        var main = particleSys.GetComponent<ParticleSystem>().main;
        main.startSize = explosionRadius/2;
    }

    private void HitDetection()
    {
        Collider[] cols = new Collider[2];

        Physics.OverlapSphereNonAlloc(transform.position, hitDetectionRadius, cols, collisionLayers, QueryTriggerInteraction.Ignore);

       for(int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
                continue;

            if (cols[i].gameObject == gameObject)
                continue;

                hit = true; //Set hit to true to exit the coroutine loop
                print(cols[0].gameObject.name);
                cols = Physics.OverlapSphere(transform.position, explosionRadius, collisionLayers, QueryTriggerInteraction.Ignore);

                for (int j = 0; j < cols.Length; j++)
                {
                    if (cols[j].GetComponent<BaseHealth>())
                    {
                        if (!Physics.Linecast(transform.position + Vector3.up * hitDetectionRadius, cols[j].transform.position + Vector3.up, 1 << LayerMask.NameToLayer("Default")))
                        {
                            cols[j].GetComponent<BaseHealth>().takeDamage(damage, gameObject);
                        }
                    }
                }
        }
        
    }

    private void OnDrawGizmos()
    {
        Color c = Color.red;
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position,hitDetectionRadius);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
