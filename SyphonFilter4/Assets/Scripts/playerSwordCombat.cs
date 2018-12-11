using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSwordCombat : MonoBehaviour {

    Animator anim;

    [SerializeField]
    private Transform sword;

    //hand that the sword attaches to
    [SerializeField]
    private Transform hand;


    //spine bone that the sword attaches to
    [SerializeField]
    private Transform spine;

    //local position and rotation values for the sword when it's parented to either bone
    private Vector3 spinePosition = new Vector3(0.00399f, 0.00789f, -0.00075f);
    private Vector3 spineEuler = new Vector3(-216.654f, -69.93298f, 21.22899f);

    private Vector3 handPosition = new Vector3(0.00077f, 0.00109f, 0.00048f);
    private Vector3 handEuler = new Vector3(-70.25201f, -12.862f, 99.54201f);

    [Header("Hit detection stuff")]

    [SerializeField]
    private Vector3 hitboxSize;

    [SerializeField]
    private Vector3 hitBoxLocation;

    [SerializeField]
    private LayerMask hitDetectionLayers;

    [SerializeField]
    private LayerMask enemyLayers;

    
    [Tooltip("Layers that can block the sword from damaging the enemy")]
    [SerializeField]
    private LayerMask blockingLayers;

    private PlayerCharacterController player;
    private PlayerHealth health;

    [Space]
    [SerializeField]
    private float damage = 15;


    private float upperBodyWeight = 0;
    public float UpperBodyWeight { set { upperBodyWeight = value; } }


    [Header("Shuriken properties")]
    [SerializeField]
    private GameObject shuriken;

    private GameObject spawnedShuriken;

    [SerializeField]
    private float shurikenTargetDistance = 20;

    [SerializeField]
    private float maxShurikenTargetAngle = 80f;

    [SerializeField]
    private int maxAmmo = 5;

    private int ammo;

    [SerializeField]
    private TrailRenderer trail;
    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();

        player = GetComponent<PlayerCharacterController>();
        health = GetComponent<PlayerHealth>();
        ammo = maxAmmo;
        ParentSwordToSpine();
    }

    public void ParentSwordToHand()
    {
        sword.SetParent(hand);
        sword.localPosition = handPosition;
        sword.localEulerAngles = handEuler;
    }

    public void ParentSwordToSpine()
    {
        sword.SetParent(spine);
        sword.localPosition = spinePosition;
        sword.localEulerAngles = spineEuler;

    }


    void Update () {

        AnimatorStateInfo layer0Info = anim.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo layer1Info = anim.GetCurrentAnimatorStateInfo(1);
        //check for input and play animations
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.CanControl && layer1Info.IsName("Idle") && anim.IsInTransition(1) == false)
            anim.SetTrigger("swordHit");
        else if (Input.GetKeyDown(KeyCode.Mouse1) && player.CanControl && player.HangingFromLedge == false && !layer0Info.IsTag("swordhit") && layer1Info.IsName("Idle") && ammo > 0)
        {
            anim.Play("shurikenThrow", 1);
            anim.SetLayerWeight(1, 0.9f);
            upperBodyWeight = 1;
        }

        anim.SetLayerWeight(1, Mathf.MoveTowards(anim.GetLayerWeight(1), upperBodyWeight, Time.deltaTime*4));
	}

    //detects the are in front of player for enemies and other objects
    public void HitDetection()
    {
        player.CanControl = true;
        Collider[] cols = new Collider[1];
        
        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitboxSize / 2,cols, transform.rotation, hitDetectionLayers);

        if(cols[0] != null)
        {
            enemyHealth e = cols[0].GetComponent<enemyHealth>();
            //if we find an enemy collider, do damage and STUN
            if (e != null)
            {

                //if there's obstacle between enemy and player, continue to next iteration
                if (Physics.Linecast(health.centerPoint, e.centerPoint, blockingLayers))
                    return;

                //stun in enemy script
                e.takeDamage(damage, gameObject);
                SoundEngine.instance.PlaySound("swordImpact", e.transform.position, null);
              
            }
        }
    }

    public void EnableSwordTrail()
    {
        SoundEngine.instance.PlaySound("sword", transform.position, transform);
        trail.emitting = true;
    }
    public void DisableSwordTrail()
    {
        trail.emitting = false;
    }
    public void DetectEnemySnap()
    {
        print("aaaa");
        Collider[] cols = new Collider[1];

        Physics.OverlapBoxNonAlloc(transform.TransformPoint(hitBoxLocation), hitboxSize, cols, transform.rotation, hitDetectionLayers);

        if (cols[0] != null)
        {
            enemyHealth e = cols[0].GetComponent<enemyHealth>();
            //if we find an enemy collider, do damage and STUN
            if (e != null)
            {
                Vector3 direction = e.centerPoint - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
                player.CanControl = false;
            }
        }
    }

    public void spawnShuriken()
    {
        ammo--;
        spawnedShuriken = (GameObject)Instantiate(shuriken, transform.position, Quaternion.identity);
        spawnedShuriken.transform.SetParent(anim.GetBoneTransform(HumanBodyBones.RightHand));
        spawnedShuriken.transform.localPosition = Vector3.zero;
        spawnedShuriken.transform.localEulerAngles = new Vector3(52.975f, -189.607f, 1.224f);
        spawnedShuriken.GetComponent<Shuriken>().enabled = false;
    }

    public void throwShuriken()
    {
       
        //check for enemies in front
        GameObject target = null;

        Collider[] enemies = Physics.OverlapSphere(transform.position, shurikenTargetDistance, enemyLayers, QueryTriggerInteraction.Ignore);

        float minimumAngle = maxShurikenTargetAngle;

        for (int i = 0; i < enemies.Length; i++)
        {

            if (Physics.Linecast(health.centerPoint, enemies[i].GetComponent<enemyHealth>().centerPoint, blockingLayers, QueryTriggerInteraction.Ignore))
                continue;

            float angle = Vector3.Angle(transform.forward, enemies[i].transform.position - transform.position);

            if(angle < minimumAngle)
            {
                minimumAngle = angle;
                target = enemies[i].gameObject;
            }
        }


        if (spawnedShuriken)
        {
            Vector3 throwDir = transform.forward;

            if (target)
                throwDir = target.GetComponent<enemyHealth>().centerPoint - spawnedShuriken.transform.position;

            spawnedShuriken.transform.SetParent(null);
            spawnedShuriken.GetComponent<Shuriken>().Initialize(throwDir);
            spawnedShuriken.GetComponent<Shuriken>().enabled = true;
            SoundEngine.instance.PlaySound("shurikenThrow", transform.position, spawnedShuriken.transform);
            spawnedShuriken = null;
        }
        else
        {
            spawnedShuriken = (GameObject)Instantiate(shuriken, transform.position, Quaternion.identity);
            spawnedShuriken.transform.SetParent(anim.GetBoneTransform(HumanBodyBones.RightHand));
            spawnedShuriken.transform.localPosition = Vector3.zero;
            spawnedShuriken.transform.localEulerAngles = new Vector3(52.975f, -189.607f, 1.224f);
            spawnedShuriken.GetComponent<Shuriken>().enabled = false;
            spawnedShuriken.transform.SetParent(null);

            Vector3 throwDir = transform.forward;

            if (target)
                throwDir = target.GetComponent<enemyHealth>().centerPoint - spawnedShuriken.transform.position;

            spawnedShuriken.GetComponent<Shuriken>().Initialize(throwDir);
            spawnedShuriken.GetComponent<Shuriken>().enabled = true;
            SoundEngine.instance.PlaySound("shurikenThrow", transform.position, spawnedShuriken.transform);
            spawnedShuriken = null;
        }
    }
    

    private void OnDrawGizmos()
    {
        Color c = Color.blue;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawCube(transform.TransformPoint(hitBoxLocation), hitboxSize);
    }
}
