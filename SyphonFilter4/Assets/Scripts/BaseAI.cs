using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BaseAI : MonoBehaviour {

    protected NavMeshAgent agent;
    protected Animator anim;

    protected Transform player = null;

    public enum AIState {NoBehaviour, Idle, Patrol, Chase};
    protected AIState state;



    protected float animatorForwardValue = 0;
    protected float upperBodyWeight = 0;

    [SerializeField]
    protected float maximumDetectDistance = 15;

    [SerializeField]
    protected float maximumDetectAngle = 90;


    protected float scanInterval = 0.5f;
    protected float lastScanTime = 0;

    //last time that the player was seen
    protected float playerSeenTime = 0;

    //how long the AI "sees" the player even through walls
    protected float playerDisappearTime = 3;

    [SerializeField]
    protected LayerMask scanningLayers;

    [SerializeField]
    protected float attackingDistance = 1.5f;

    [SerializeField]
    protected float stoppingDistance = 1.5f;


    [SerializeField]
    protected float rotateSpeed = 400;



    //hitbox stuff

        [Header("Hit-detection")]
        [Space()]
    [SerializeField]
    protected Vector3 hitBoxSize;
    [SerializeField]
    protected Vector3 hitBoxLocation;

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    [SerializeField]
    protected float damage = 10;

    [SerializeField]
    private float chargeDamage = 20;

    private bool charging = false;

    private float lastChargeTime;

    private float chargeIntervalMin = 5, chargeIntervalMax = 10;
    private float chargeDuration = 2;
    private ParticleSystem chargeParticle;

    [SerializeField]
    private float chargeSpeed = 10;

    //did the player aggro the enemy by shooting it
    protected bool aggroed = false;
    protected float lastAggroTime;
    protected float aggroTimer = 5f;
    public float LastAggroTime { get { return lastAggroTime; }set { lastAggroTime = value; aggroed = true; } }

	// Use this for initialization
	protected virtual void Start () {
        anim = GetComponent<Animator>();
        player = PlayerCharacterController.player.transform;
        agent = GetComponent<NavMeshAgent>();
        playerSeenTime = -playerDisappearTime - 1;
        lastChargeTime += Random.Range(chargeIntervalMin, chargeIntervalMax);

        ChangeState(AIState.Idle);

        chargeParticle = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {

        UpdateAnimatorValues();


        if (state == AIState.NoBehaviour)
        {
            //stay still do nothing
        }
        else if (state == AIState.Idle)
        {
            //stay still scan for player
            if (player)
            {
                Idle();
            }
            else
            ChangeState(AIState.NoBehaviour);
        }

        else if (state == AIState.Chase)
        {

            //move towards player if can't go to idle
            if (player)
            {
                Chase();
            }
            else ChangeState(AIState.NoBehaviour);
        }
        else if (state == AIState.Patrol)
        {

            //move between points and scan for player
        }


	}

    protected virtual void NoBehaviour()
    {
        animatorForwardValue = 0;

    }

    protected virtual bool CanAttack()
    {
        return !anim.IsInTransition(1);

    }

    public void MeleeHitDetection()
    {

        //first collider found with overlapbox
        Collider[] c = new Collider[1];
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers);

        if (c[0] != null)
        {

            if (c[0].GetComponent<BaseHealth>())
            {
                c[0].GetComponent<BaseHealth>().takeDamage(damage, gameObject);
                if(charging)
                {
                    lastChargeTime = Time.time + Random.Range(chargeIntervalMin, chargeIntervalMax);
                    charging = false;
                }
            
            }
        }
        
    }

    private bool ChargeHitDetection(out BaseHealth b)
    {
        b = null;
        //first collider found with overlapbox
        Collider[] c = new Collider[1];
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitBoxSize / 2, c, transform.rotation, hitDetectionLayers);

        if (c[0] != null)
        {

            if (c[0].GetComponent<BaseHealth>())
            {
                b = c[0].GetComponent<BaseHealth>();
                return true;
               
            }
        }

        return false;

    }
    protected virtual void Idle()
    {
        animatorForwardValue = 0;
        bool playerDetected = aggroed && Time.time < lastAggroTime + aggroTimer;

        if (!playerDetected)
            playerDetected = IsPlayerDetected();

        if (playerDetected)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                ChangeState(AIState.Chase);
            }
            else
            {
                Vector3 direction = player.position - transform.position;
                direction.y = 0;
                Quaternion lookDir = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);

                if (CanAttack())
                {
                    upperBodyWeight = 1;
                    CancelInvoke();
                    anim.SetTrigger("attack");
                    Invoke("ResetUpperBodyWeight", 1.5f);
                }
                  
            }
        }

    }

    void ResetUpperBodyWeight()
    {
        upperBodyWeight = 0;
    }

    protected virtual void UpdateAnimatorValues()
    {
        anim.SetFloat("Forward", animatorForwardValue, 0.2f, Time.deltaTime);
        anim.SetBool("charging", charging);

        if(upperBodyWeight == 0)
        anim.SetLayerWeight(1, Mathf.MoveTowards(anim.GetLayerWeight(1), upperBodyWeight, Time.deltaTime * 2));
        else
            anim.SetLayerWeight(1, Mathf.MoveTowards(anim.GetLayerWeight(1), upperBodyWeight, Time.deltaTime * 6));
    }
    protected virtual void Chase()
    {

        bool playerDetected = IsPlayerDetected();

        if (playerDetected)
        {
            agent.destination = player.position;

            //player is unreachable or destination is invalid
            if (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                print("path not compelte");
                ChangeState(AIState.Idle);
                return;
            }

            animatorForwardValue = 1;
            Vector3 direction = agent.steeringTarget - transform.position;
            direction.y = 0;
            Quaternion lookDir = Quaternion.LookRotation(direction);
           
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (Time.time > lastChargeTime && !charging)
            {
                chargeParticle.Play();
                charging = true;
                lastChargeTime = Time.time;

            }

            //CHARGE
            if (charging)
            {
                agent.velocity = transform.forward * chargeSpeed;
                BaseHealth b;
                if (ChargeHitDetection(out b))
                {
                    b.takeDamage(chargeDamage, gameObject);
                    StopCharge();
                    lastChargeTime = Time.time + Random.Range(chargeIntervalMin, chargeIntervalMax);
                    
                }

                if (Time.time > lastChargeTime + chargeDuration)
                {
                    StopCharge();
                    lastChargeTime = Time.time + Random.Range(chargeIntervalMin, chargeIntervalMax);
                }
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);
            }
            if (distanceToPlayer <= stoppingDistance && !charging)
            {
                ChangeState(AIState.Idle);

            }


        }
        else
        {
            ChangeState(AIState.Idle);
        }
    }

    private void StopCharge()
    {
        charging = false;
        chargeParticle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }


    //checks if the player is within distance and angle and that there's no collisions between them
    protected virtual bool IsPlayerDetected()
    {
        if (aggroed && Time.time < lastAggroTime + aggroTimer)
        {
            return true;
        }

        if (Time.time > lastScanTime + scanInterval)
        {
            lastScanTime = Time.time;
            if (Vector3.Distance(transform.position, player.position) <= maximumDetectDistance)
            {
                if (Vector3.Angle(transform.forward, player.position - transform.position) < maximumDetectAngle)
                {
                    //1.5f
                    RaycastHit hit;
                    Ray ray = new Ray(transform.TransformPoint(0f, 1.5f * transform.localScale.x, 0f), player.position - transform.position);
                    Debug.DrawRay(ray.origin, ray.direction.normalized * maximumDetectDistance);
                    if (Physics.Raycast(ray, out hit, maximumDetectDistance, scanningLayers))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            playerSeenTime = Time.time;
                            return true;
                        }

                    }
                    else print("no raycast");
                }
            }
            
            return false;
        }


        if (Time.time > playerSeenTime + playerDisappearTime)
        {
            return false;
        }

        return true;
    }


    public virtual void ChangeState(AIState state)
    {

        this.state = state;


    }

    protected virtual void OnAnimatorMove()
    {
        if (agent.enabled && !agent.isStopped)
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsTag("rootmotion"))
            agent.velocity = anim.deltaPosition / Time.deltaTime;
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
