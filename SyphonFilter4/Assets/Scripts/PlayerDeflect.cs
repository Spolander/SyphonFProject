using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour {

    [SerializeField]
    ParticleSystem DeflectParticle;

    [Header("Deflection detection")]
    [Space()]
    [SerializeField]
    protected Vector3 hitBoxSize;
    [SerializeField]
    protected Vector3 hitBoxLocation;

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    private float lastDeflectTime;

    GameObject deflectable;

    [SerializeField]
    float DeflectSpeed;

    [SerializeField]
    Transform[] DeflectPoints;

    float deflectTime = 0.2f;

    void Start () {
        DeflectParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("deflect"))
        {
            // deflect timer here
                Debug.Log("PushingE");
                if (DeflectHitDetection())
                {
                    Debug.Log("DetectedHit");
                    //spawn deflect particles here

                    //set direction of deflectable object towards random deflectpoint
                    if (deflectable.GetComponent<enemyThrowable>())
                    {
                        deflectable.GetComponent<enemyThrowable>().Initialize(DeflectPoints[Random.Range(0, 5)].position, 0.5f, 10);
                    }

                    //deflectable.transform.position = Vector3.MoveTowards(deflectable.transform.position, DeflectPoints[Random.Range(0, 5)].position, DeflectSpeed*Time.deltaTime);

                    //draw traces
                }
        }
    }

    public bool DeflectHitDetection()
    {
        //first collider found with overlapbox
        Collider[] c = new Collider[1];
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers, QueryTriggerInteraction.Ignore);

        if (c[0] != null)
        {
            Debug.Log("NOTNULL");
            if (c[0].GetComponent<Deflectable>())
            {
                Debug.Log("ISDEFLECTABLE");
                deflectable = c[0].gameObject;
                return true;
            }
            
        }
        return false;

    }



    private void OnDrawGizmosSelected()
    {
        Color c = Color.red;
        c.a = 0.4f;

        Gizmos.color = c;

        Gizmos.DrawCube(transform.TransformPoint(hitBoxLocation), hitBoxSize);
    }
}
