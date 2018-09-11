using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MechaAI:BaseAI{

    private bool isWalking = false;

    [SerializeField]
    private Transform leftGunBase;
    [SerializeField]
    private Transform rightGunBase;

    [SerializeField]
    private Transform rightGunBarrel;

    [SerializeField]
    private Transform leftGunBarrel;


    //original localrotations for the guns
    Quaternion originalBaseRot_L;
    Quaternion originalBaseRot_R;

    Quaternion originalGunRot_L;
    Quaternion originalGunRot_R;

    [SerializeField]
    private float weaponRotateSpeed = 30;

    protected override void Start()
    {
        base.Start();


        //get all weapon original local rotations
        originalBaseRot_L = leftGunBase.localRotation;
        originalBaseRot_R = rightGunBase.localRotation;

        originalGunRot_L = leftGunBarrel.localRotation;
        originalGunRot_R = rightGunBarrel.localRotation;
    }
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

            if (distanceToPlayer < attackingDistance)
            {
                //SHOOT KILL DESTROY
            }
        }

       
    }

    protected override void Update()
    {
        RotateWeapons();
        base.Update();
    }

    void RotateWeapons()
    {
        //horizontal distance to the player
        float horizontalDistance = Vector3.Distance(player.position, new Vector3(transform.position.x, player.position.y, transform.position.z));

        //Vertical distance to the player from the gun position
        float verticalDistance = (rightGunBase.position - player.position).y;


        //calculate vertical angle with arc tan
        float verticalAngle = Mathf.Atan(verticalDistance / horizontalDistance) * Mathf.Rad2Deg;


       

        //if player is in front of the mech we rotate all the guns properly towards player
        if (transform.InverseTransformPoint(player.position).z > 0.5f)
        {
            rightGunBarrel.rotation = Quaternion.RotateTowards(rightGunBarrel.rotation, Quaternion.FromToRotation(rightGunBarrel.up, player.position - rightGunBarrel.position) * rightGunBarrel.rotation, Time.deltaTime * weaponRotateSpeed);
            leftGunBarrel.rotation = Quaternion.RotateTowards(leftGunBarrel.rotation, Quaternion.FromToRotation(leftGunBarrel.up, player.position - leftGunBarrel.position) * leftGunBarrel.rotation, Time.deltaTime * weaponRotateSpeed);

            rightGunBase.localRotation = Quaternion.RotateTowards(rightGunBase.localRotation, Quaternion.Euler(verticalAngle, rightGunBase.localEulerAngles.y, rightGunBase.localEulerAngles.z), Time.deltaTime*weaponRotateSpeed);
            leftGunBase.localRotation = Quaternion.RotateTowards(leftGunBase.localRotation,Quaternion.Euler(verticalAngle, leftGunBase.localEulerAngles.y, leftGunBase.localEulerAngles.z),Time.deltaTime*weaponRotateSpeed);

        }

        //player is behind so we rotate the weapons back to the default poses
        else
        {
            rightGunBase.localRotation = Quaternion.RotateTowards(rightGunBase.localRotation, originalBaseRot_R, Time.deltaTime * weaponRotateSpeed);
            leftGunBase.localRotation = Quaternion.RotateTowards(leftGunBase.localRotation, originalBaseRot_L, Time.deltaTime * weaponRotateSpeed);

            rightGunBarrel.localRotation = Quaternion.RotateTowards(rightGunBarrel.localRotation, originalGunRot_R, Time.deltaTime * weaponRotateSpeed);
            leftGunBarrel.localRotation = Quaternion.RotateTowards(leftGunBarrel.localRotation, originalGunRot_L, Time.deltaTime * weaponRotateSpeed);
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
