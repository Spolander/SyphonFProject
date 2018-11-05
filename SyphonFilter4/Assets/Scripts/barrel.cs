using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrel : MonoBehaviour {

    [SerializeField]
    int ExplosionDamage;

    [SerializeField]
    private float hitDetectionRadius = 1f;

    [SerializeField]
    private float ExplosionRadius = 4;

    [SerializeField]
    private GameObject explosionEffect;

    [SerializeField]
    private LayerMask collisionLayers;

    bool exploded;

    bool hit = false;


    private void HitDetection()
    {
        Collider[] cols = new Collider[2];

        Physics.OverlapSphereNonAlloc(transform.position, hitDetectionRadius, cols, collisionLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
                continue;

            if (cols[i].gameObject == gameObject)
                continue;

            hit = true; //Set hit to true to exit the coroutine loop
            print(cols[0].gameObject.name);
            cols = Physics.OverlapSphere(transform.position, ExplosionRadius, collisionLayers, QueryTriggerInteraction.Ignore);

            for (int j = 0; j < cols.Length; j++)
            {
                if (cols[j].GetComponent<BaseHealth>() && cols[j].gameObject != gameObject)     //if the object has health and is not this object
                {
                    if (!Physics.Linecast(transform.position + Vector3.up * hitDetectionRadius, cols[j].transform.position + Vector3.up, 1 << LayerMask.NameToLayer("Default")))
                    {
                        if (cols[j].GetComponent<barrel>())
                        {
                            if (cols[j].GetComponent<barrel>().exploded == false)
                            {
                                exploded = true;
                                cols[j].GetComponent<BaseHealth>().takeDamage(ExplosionDamage, gameObject);  
                            }
                        }
                        else
                        {
                            cols[j].GetComponent<BaseHealth>().takeDamage(ExplosionDamage, gameObject);
                        }
                    }
                }
            }
        }

    }
    public void Explode()
    {
        GameObject particleSys = (GameObject)Instantiate(explosionEffect, transform.position, Quaternion.identity);
        var main = particleSys.GetComponent<ParticleSystem>().main;
        main.startSize = ExplosionRadius / 2;
        HitDetection();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
