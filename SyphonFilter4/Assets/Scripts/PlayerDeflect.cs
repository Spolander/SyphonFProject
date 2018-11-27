using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour {

    [SerializeField]
    private GameObject DeflectionEffect;
    [SerializeField]
    private float deflectionRadius = 1;

    [Header("Deflection detection")]
    [Space()]
    [SerializeField]
    protected Vector3 hitBoxSize;
    [SerializeField]
    protected Vector3 hitBoxLocation;

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    private float lastDeflectTime;

    GameObject[] deflectable = new GameObject [10];

    [SerializeField]
    float DeflectSpeed;

    [SerializeField]
    Transform[] DeflectPoints;

    float deflectTime = 0.2f;

    void Start () {
    }

    private void Update()
    {
        if (Input.GetButtonDown("deflect"))
        {
            // deflect timer here
                if (DeflectHitDetection())
                {
                for (int i = 0; i < deflectable.Length; i++)
                {
                    if (deflectable[i].GetComponent<enemyThrowable>())
                    {
                        Debug.Log("Throwable deflected");
                        //set direction of deflectable object towards random deflectpoint
                        deflectable[i].GetComponent<enemyThrowable>().Initialize(DeflectPoints[Random.Range(0, 5)].position, 0.5f, 10);
                        //deflection particles
                        GameObject particleSys = (GameObject)Instantiate(DeflectionEffect, deflectable[i].transform.position, Quaternion.identity);
                    }

                    else if (deflectable[i].GetComponent<EnemyProjectile>())
                    {
                        Debug.Log("particle deflected");
                        //set direction of deflectable object towards random deflectpoint
                        deflectable[i].GetComponent<EnemyProjectile>().Initialize(DeflectPoints[Random.Range(0, 5)].position, 0.5f, 10);
                        //deflection particles
                        GameObject particleSys = (GameObject)Instantiate(DeflectionEffect, deflectable[i].transform.position, Quaternion.identity);
                    }
                    //draw traces
                }
                }
        }
    }

    public bool DeflectHitDetection()
    {
        //first collider found with overlapbox
        Collider[] c = new Collider[10];
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] != null)
            {
                Debug.Log("NOTNULL");
                if (c[i].GetComponent<Deflectable>())
                {
                    Debug.Log("ISDEFLECTABLE");
                    deflectable[i] = c[i].gameObject;
                    
                }

            }
        }
        return true;
    }



    private void OnDrawGizmosSelected()
    {
        Color c = Color.red;
        c.a = 0.4f;

        Gizmos.color = c;

        Gizmos.DrawCube(transform.TransformPoint(hitBoxLocation), hitBoxSize);
    }
}
