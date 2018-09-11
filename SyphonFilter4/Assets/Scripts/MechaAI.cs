using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MechaAI:BaseAI{

    private bool isWalking = false;

    protected override void UpdateAnimatorValues()
    {
        anim.SetBool("isWalking", isWalking);
    }

    protected override void Idle()
    {
        
    
        if (IsPlayerDetected())
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                isWalking = true;
                ChangeState(AIState.Chase);
            }
            else
            {
                isWalking = false;

            }
        }

       
    }

    protected override void Chase()
    {
        isWalking = true;
        if (IsPlayerDetected())
        {
            agent.destination = player.position;

            //player is unreachable or destination is invalid
            if (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                print("path not compelte");
                ChangeState(AIState.Idle);
                return;
            }

            Vector3 direction = agent.steeringTarget - transform.position;
            direction.y = 0;
            Quaternion lookDir = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= stoppingDistance)
            {
                ChangeState(AIState.Idle);

            }


        }
        else
        {
            ChangeState(AIState.Idle);
        }
    }

    public override void ChangeState(AIState state)
    {
        base.ChangeState(state);
        if (state == AIState.Idle)
        {
            isWalking = false;
        }
    }
    protected override void OnAnimatorMove()
    {
        Vector3 deltaMovement = anim.deltaPosition / Time.deltaTime;
        deltaMovement = Vector3.ProjectOnPlane(deltaMovement, Vector3.up);

        agent.velocity = deltaMovement;
    }

}
