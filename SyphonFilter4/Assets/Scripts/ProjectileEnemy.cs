using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class ProjectileEnemy : BaseAI {

    private float lastAttackTime;

    [SerializeField]
    private float attackingInterval = 3;

    [SerializeField]
    private GameObject projectile;

    private float speed;
    private float ProjDamage;


    protected override void Start()
    {
        base.Start();
        lastAttackTime -= attackingInterval;

        speed = GetComponent<EnemyProjectile>().projectileSpeed;
        ProjDamage = GetComponent<EnemyProjectile>().damage;

        Debug.Log(speed + "damage" + ProjDamage);
    }

    public void projectileAttack()
    {
        if (player == null)
            return;

        GameObject g = (GameObject)Instantiate(projectile, anim.GetBoneTransform(HumanBodyBones.RightHand).position, Quaternion.Euler(0,0,90));
        g.GetComponent<EnemyProjectile>().Initialize(player.position, speed, ProjDamage);

        //PlayerCharacterController.player.transform.position
    }
    protected override void Idle()
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
                    CancelInvoke();
                    anim.SetTrigger("attack");
                }
            }
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
                print("path not compelte");
                ChangeState(AIState.Idle);
                return;
            }
            animatorForwardValue = 1;
            Vector3 direction = agent.steeringTarget - transform.position;
            direction.y = 0;
            Quaternion lookDir = Quaternion.LookRotation(direction);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDir, Time.deltaTime * rotateSpeed);

            if (distanceToPlayer < attackingDistance)
            {
                if (CanAttack())
                {
                    CancelInvoke();
                    anim.SetTrigger("attack");
                }
            }

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

    protected override void UpdateAnimatorValues()
    {
        anim.SetFloat("Forward", animatorForwardValue, 0.2f, Time.deltaTime);
    }
    protected override bool CanAttack()
    {
        return Time.time > lastAttackTime + attackingInterval && anim.GetCurrentAnimatorStateInfo(0).IsName("Move");
    }
}

