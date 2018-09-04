using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BaseAI : MonoBehaviour {

    NavMeshAgent agent;
    Animator anim;

    Transform player = null;

    public enum AIState {NoBehaviour, Idle, Patrol, Chase};
    private AIState state;



    protected float animatorForwardValue = 0;


    protected float maximumDetectDistance = 15;
    protected float maximumDetectAngle = 90;


    protected float scanInterval = 0.5f;
    protected float lastScanTime = 0;

    //last time that the player was seen
    protected float playerSeenTime = 0;

    //how long the AI "sees" the player even through walls
    protected float playerDisappearTime = 3;

    [SerializeField]
    protected LayerMask scanningLayers;
    
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        player = PlayerCharacterController.player.transform;
        agent = GetComponent<NavMeshAgent>();
        playerSeenTime = -playerDisappearTime - 1;

        ChangeState(AIState.Idle);
	}
	
	// Update is called once per frame
	void Update ()
    {
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
            else ChangeState(AIState.NoBehaviour);
        }

        else if (state == AIState.Chase)
        {

            //move towards player if can't go to idle
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

    protected virtual void Idle()
    {
        animatorForwardValue = 0;

        if (IsPlayerDetected())
        {
            print("I can see you");
        }
        else print("I can't see you");

    }


    //checks if the player is within distance and angle and that there's no collisions between them
    bool IsPlayerDetected()
    {
        
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


    public void ChangeState(AIState state)
    {

        this.state = state;


    }

    private void OnAnimatorMove()
    {
        if (agent.enabled && !agent.isStopped)
        {
            agent.velocity = anim.deltaPosition / Time.deltaTime;
        }
    }
}
