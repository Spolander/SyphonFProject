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


    //Shooting stuff
    [SerializeField]
    private float weaponRotateSpeed = 30;

    [SerializeField]
    private Transform[] cannons;

    private float lastFireTime;
    [SerializeField]
    private float shootingInterval = 0.2f;

    [SerializeField]
    private float projectileSpeed = 75;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject bulletImpactPrefab;

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

            if (distanceToPlayer <= attackingDistance)
            {
                shootCannons();
            }

            if (distanceToPlayer > stoppingDistance)
            {
                ChangeState(AIState.Chase);
            }
        }
        RotateWeapons();
       
    }

    protected override void Update()
    {
    
        base.Update();
    }

    void RotateWeapons()
    {
        if (player)
        {
            Vector3 playerPosition = player.position + Vector3.up*0.5f;
            //horizontal distance to the player
            float horizontalDistance = Vector3.Distance(playerPosition, new Vector3(transform.position.x, playerPosition.y, transform.position.z));

            //Vertical distance to the player from the gun position
            float verticalDistance = (rightGunBase.position - playerPosition).y;


            //calculate vertical angle with arc tan
            float verticalAngle = Mathf.Atan(verticalDistance / horizontalDistance) * Mathf.Rad2Deg;




            //if player is in front of the mech we rotate all the guns properly towards player
            if (transform.InverseTransformPoint(playerPosition).z > 0.5f)
            {
                rightGunBarrel.rotation = Quaternion.RotateTowards(rightGunBarrel.rotation, Quaternion.FromToRotation(rightGunBarrel.up, playerPosition - rightGunBarrel.position) * rightGunBarrel.rotation, Time.deltaTime * weaponRotateSpeed);
                leftGunBarrel.rotation = Quaternion.RotateTowards(leftGunBarrel.rotation, Quaternion.FromToRotation(leftGunBarrel.up, playerPosition - leftGunBarrel.position) * leftGunBarrel.rotation, Time.deltaTime * weaponRotateSpeed);

                rightGunBase.localRotation = Quaternion.RotateTowards(rightGunBase.localRotation, Quaternion.Euler(verticalAngle, rightGunBase.localEulerAngles.y, rightGunBase.localEulerAngles.z), Time.deltaTime * weaponRotateSpeed);
                leftGunBase.localRotation = Quaternion.RotateTowards(leftGunBase.localRotation, Quaternion.Euler(verticalAngle, leftGunBase.localEulerAngles.y, leftGunBase.localEulerAngles.z), Time.deltaTime * weaponRotateSpeed);

            }
            else
            {
                rightGunBase.localRotation = Quaternion.RotateTowards(rightGunBase.localRotation, originalBaseRot_R, Time.deltaTime * weaponRotateSpeed);
                leftGunBase.localRotation = Quaternion.RotateTowards(leftGunBase.localRotation, originalBaseRot_L, Time.deltaTime * weaponRotateSpeed);

                rightGunBarrel.localRotation = Quaternion.RotateTowards(rightGunBarrel.localRotation, originalGunRot_R, Time.deltaTime * weaponRotateSpeed);
                leftGunBarrel.localRotation = Quaternion.RotateTowards(leftGunBarrel.localRotation, originalGunRot_L, Time.deltaTime * weaponRotateSpeed);
            }
        }
        //player doesn't exist so we rotate the weapons back to the default poses
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
        
        if (IsPlayerDetected())
        {
           
            agent.destination = player.position;

            //player is unreachable or destination is invalid
            if (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                ChangeState(AIState.Idle);
                return;
            }

            Vector3 direction = agent.steeringTarget - transform.position;
            direction.y = 0;
            Quaternion lookDir = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < stoppingDistance-1)
            {
                ChangeState(AIState.Idle);

            }

            if (distanceToPlayer <= attackingDistance)
            {
                shootCannons();
            }

        }
        else
        {
            ChangeState(AIState.Idle);
        }

        RotateWeapons();
    }

    private void shootCannons()
    {
        if (Time.time > lastFireTime + shootingInterval && transform.InverseTransformPoint(player.position).z > 0.3f)
        {
            lastFireTime = Time.time;

            for (int i = 0; i < cannons.Length; i++)
            {
                RaycastHit hit;
                Vector3 endPoint = cannons[i].position + cannons[i].forward * attackingDistance;
                Ray ray = new Ray(cannons[i].position,  cannons[i].forward);

                float defaultLife = 0.5f;

                if (Physics.Raycast(ray, out hit, attackingDistance, hitDetectionLayers))
                {
                    defaultLife = hit.distance / projectileSpeed;
                    GameObject e = (GameObject)Instantiate(bulletImpactPrefab, hit.point, Quaternion.identity);
                    e.transform.rotation = Quaternion.FromToRotation(e.transform.up, hit.normal);
                    ParticleSystem p = e.GetComponent<ParticleSystem>();
                    var m = p.main;
                    m.startDelay = hit.distance / projectileSpeed;
                    p.Play();
                    endPoint = hit.point;
                }


                GameObject g = (GameObject)Instantiate(bulletPrefab, cannons[i].position, Quaternion.identity);
                g.GetComponent<MechaProjectile>().Initialize(endPoint, cannons[i].forward, projectileSpeed, Mathf.Clamp(defaultLife, 0.2f, 10), 5);
            }
        }
    }

    public override void ChangeState(AIState state)
    {
        base.ChangeState(state);

        if (state == AIState.Idle || state == AIState.NoBehaviour)
            isWalking = false;
        else if (state == AIState.Chase)
            isWalking = true;
       
    }
    protected override void OnAnimatorMove()
    {
        Vector3 deltaMovement = anim.deltaPosition / Time.deltaTime;
        deltaMovement = Vector3.ProjectOnPlane(deltaMovement, Vector3.up);

        agent.velocity = deltaMovement;
    }

}
