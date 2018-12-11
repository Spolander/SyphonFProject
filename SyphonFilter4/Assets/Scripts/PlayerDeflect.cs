using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeflect : MonoBehaviour {

    Animator anim;

    [SerializeField]
    private GameObject DeflectionEffect;

    //box where we check for deflectable projectiles
    [Header("Deflection detection")]
    [Space()]
    [SerializeField]
    protected Vector3 hitBoxSize;
    [SerializeField]
    protected Vector3 hitBoxLocation;

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    GameObject[] deflectable = new GameObject [10];

    //speed for projectile after deflection
    [SerializeField]
    float DeflectSpeed;

    //points where projectile goes after deflection
    [SerializeField]
    Transform[] DeflectPoints;

    //time window where projectiles collider check happens
    [SerializeField]
    float deflectTimeWindow = 0.5f;
    //penalty time for failed deflection
    [SerializeField]
    float deflectFailPenalty = 1.5f;
    //time of last attempted deflect
    float deflectStart;

    public bool deflectFail=false;

    void Start () {
        deflectStart = -deflectTimeWindow;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("deflect") && deflectFail == false)
        {
            //deflect animation
            if(GetComponent<PlayerCharacterController>().isGrounded && !GetComponent<PlayerCharacterController>().HangingFromLedge)
            {
                if (Random.Range(1, 3) == 1)
                {
                    anim.SetTrigger("deflect");
                    Debug.Log("def1");
                }
                else
                {
                    anim.SetTrigger("deflect2");
                    Debug.Log("def2");
                }
                deflectStart = Time.time;
            }

        }
        if (Time.time < deflectStart + deflectTimeWindow)
        {
            //within deflect time window, check for projectile colliders. If found, add them to deflectable array
            DeflectHitDetection();

            //check the array
            for (int i = 0; i < deflectable.Length; i++)
            {

                //if something was added to the array, deflect is success
                if (deflectable[i] != null)
                {
                    //if the object is a grenade and is not deflected before
                    if (deflectable[i].GetComponent<enemyThrowable>() && deflectable[i].GetComponent<Deflectable>().isDeflected != true)
                    {
                        Debug.Log("Throwable deflected");
                        //set direction of deflectable object towards random deflectpoint
                        deflectable[i].GetComponent<enemyThrowable>().Initialize(DeflectPoints[Random.Range(0, 5)].position, 0.5f, 10);
                        //set deflected boolean to true, so that it cannot be deflected again
                        deflectable[i].GetComponent<Deflectable>().isDeflected = true;
                        //deflection particles
                        GameObject particleSys = (GameObject)Instantiate(DeflectionEffect, deflectable[i].transform.position, Quaternion.identity);
                    }

                    //if the object is a projectile and is not deflected before
                    else if (deflectable[i].GetComponent<EnemyProjectile>() && deflectable[i].GetComponent<Deflectable>().isDeflected != true)
                    {
                        Debug.Log("particle deflected");
                        //set direction of deflectable object towards random deflectpoint
                        deflectable[i].GetComponent<EnemyProjectile>().Initialize(transform.TransformPoint(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)))); //, deflectable[i].GetComponent<EnemyProjectile>().projectileSpeed, 50);
                        //set deflected boolean to true, so that it cannot be deflected again
                        deflectable[i].GetComponent<Deflectable>().isDeflected = true;
                        //deflection particles
                        GameObject particleSys = (GameObject)Instantiate(DeflectionEffect, deflectable[i].transform.position, Quaternion.identity);
                    }
                    //draw traces if needed
                }
                //if tried to deflect nothing; deflect is a fail
                else
                {
                    deflectFail = true;
                }
            }
        }
        //reset deflect fail when penalty time is over
        if(Time.time > deflectStart + deflectFailPenalty && deflectFail == true)
        {
            deflectFail = false;
            Debug.Log("usable");
        }
    }

    public void DeflectHitDetection()
    {
        //check all ocjects in the overlapbox if they are deflectable
        Collider[] c = new Collider[10];
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers, QueryTriggerInteraction.Collide);
        
        //Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers, QueryTriggerInteraction.Collide); 
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] != null)
            {
                //if object is deflectable, add it to the array
                if (c[i].GetComponent<Deflectable>())
                {
                    deflectable[i] = c[i].gameObject;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color c = Color.red;
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawCube(transform.TransformPoint(hitBoxLocation), hitBoxSize);
    }
}
