using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCombat : MonoBehaviour {

    IKController ikc;
    Animator anim;
    Camera mainCam;

    BaseHealth EnemyHealth;             // damage testiä varten
    [SerializeField]
    int GunDamage = 34;                    // damage testiä varten
    RaycastHit ShootRaycastHit;         // mihin tähdätään laukaisuhetkellä
    [SerializeField]
    float GunFireRate =0.5f;            // aika damagen dealauksen välissä
    bool damageDone = false;
    float damageTimer=0;

    [SerializeField]
    LayerMask EnemyLayerMask = 10;
   

    [SerializeField]
    private float maxFreeAimShootingAngle = 60f;

    [SerializeField]
    private float maxLockOnAngle = 90f;

    [SerializeField]
    private float maxShootingDistance = 15;
    // Use this for initialization

    private bool canControl = true;


    float lastFreeAimScan;
    float freeAimScanInterval = 0.2f;

    [SerializeField]
    private LayerMask lockOnLayers;

    [SerializeField]
    private LayerMask lockOnBlockingLayers;

    private Transform currentFreeAimingTarget = null;

    private MuzzleFlashAnimation muzzles;

    [SerializeField]
    private GameObject projectileImpact;
	void Start () {
        muzzles = GetComponent<MuzzleFlashAnimation>();
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        ikc = GetComponent<IKController>();
    }
	
	// Update is called once per frame
	void Update () {

        if (damageDone == true)
        {
            damageTimer += Time.deltaTime;
        }
        if (damageTimer > GunFireRate && damageDone == true)
        {
            damageDone = false;
            damageTimer = 0;
        }


        if (canControl == false)
            return;

        

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (ikc.Target)
            {
                stopLockOn();
            }

            else
            {
                scanForLockOn();
            }

        }

        Shooting();
    }

    private void Shooting()
    {

        ikc.Shooting = Input.GetKey(KeyCode.Mouse0);

        if (ikc.Shooting)
        {
            if (ikc.LockedOn && damageDone == false)
            {
                damageDone = true;
                //shoot towards target

                if (ikc.CurrentLookAtWeight > 0.8f)
                    if (Physics.Raycast(transform.TransformPoint(0f, 1, 0f), ikc.Target.transform.TransformPoint(0f, 1.4f, 0f) - transform.TransformPoint(0f, 1, 0f), out ShootRaycastHit, maxShootingDistance, EnemyLayerMask))
                    {
                       GameObject g =  Instantiate(projectileImpact, ShootRaycastHit.point, Quaternion.identity) as GameObject;
                        g.transform.SetParent(ShootRaycastHit.collider.transform);

                        if (ShootRaycastHit.collider.GetComponent<BaseHealth>())
                        {
                            EnemyHealth = ikc.Target.GetComponent<BaseHealth>();
                                EnemyHealth.takeDamage(damage, gameObject);
                        }
                    }



            }
            else if(damageDone == false)
            {
                damageDone = true;
                scanForFreeAim();

                if (currentFreeAimingTarget)
                {
                    ikc.Target = currentFreeAimingTarget;
                    if (ikc.CurrentLookAtWeight > 0.8f)
                        if (Physics.Raycast(transform.TransformPoint(0f, 1, 0f), ikc.Target.transform.TransformPoint(0f, 1.4f, 0f) - transform.TransformPoint(0f, 1, 0f), out ShootRaycastHit, maxShootingDistance, EnemyLayerMask))
                        {
                            GameObject g = Instantiate(projectileImpact, ShootRaycastHit.point, Quaternion.identity) as GameObject;
                            g.transform.SetParent(ShootRaycastHit.collider.transform);

                            if (ShootRaycastHit.collider.GetComponent<BaseHealth>())
                            {
                                EnemyHealth = ikc.Target.GetComponent<BaseHealth>();
                                EnemyHealth.takeDamage(damage, gameObject);
                               
                            }
                        }
                }
                else
                {
                    if (ikc.CurrentLookAtWeight > 0.8f)
                        if (Physics.Raycast(transform.TransformPoint(0f, 1.5f, 0f), transform.forward, out ShootRaycastHit, maxShootingDistance, EnemyLayerMask))
                        {
                            Instantiate(projectileImpact, ShootRaycastHit.point, Quaternion.identity);
                        }

                    ikc.Target = null;
                }
            }

            if (ikc.CurrentLookAtWeight > 0.8f)
                muzzles.Animate();
        }
        else
        {
            currentFreeAimingTarget = null;
            if (!ikc.LockedOn)
            {
                ikc.Target = null;
            }

            if (muzzles.Animating)
                muzzles.StopAnimating();
        }
    }

    public void stopLockOn()
    {
        //set the ikcontroller target to null and lockedOn to false to stop ik 
        ikc.Target = null;
        ikc.LockedOn = false;

        //for animator transition back to normal
        anim.SetBool("lockon", false);

        //set the camera follow rotation angle to current rotation y
        CameraFollow.playerCam.RotationAngleY = mainCam.transform.eulerAngles.y - 180;
        CameraFollow.playerCam.LockOnTarget = null;
    }


    //scans for aiming target from camera view direction
    void scanForLockOn()


    {

            Transform target = null;
            Collider[] cols = Physics.OverlapSphere(transform.position, maxShootingDistance, lockOnLayers);
            float maxDistance = Mathf.Infinity;
            for (int i = 0; i < cols.Length; i++)
            {
            if (!Physics.Linecast(transform.TransformPoint(0, 1.5f, 0), cols[i].transform.TransformPoint(0f, 1f, 0), lockOnBlockingLayers))
            {

                float distance = Vector3.Distance(transform.position, cols[i].transform.position);
                if (distance < maxDistance)
                {
                    float angle = Vector3.Angle(mainCam.transform.forward, cols[i].transform.position - transform.position);

                    if (angle < maxLockOnAngle)
                    {
                        target = cols[i].transform;
                        maxDistance = distance;
                    }
                }
            }
            }


            if (target != null)
            {
            anim.SetBool("lockon", true);
            ikc.Target = target;
            ikc.LockedOn = true;
            CameraFollow.playerCam.LockOnTarget = target;
            }
        
    }

    //scan for shooting target when free aiming 
    void scanForFreeAim()
    {

        if (Time.time > lastFreeAimScan + freeAimScanInterval)
        {
            lastFreeAimScan = Time.time;

            Transform target = null;

            //Get all colliders around player
            Collider[] cols = Physics.OverlapSphere(transform.position, maxShootingDistance, lockOnLayers);
            float maxDistance = Mathf.Infinity;
            for (int i = 0; i < cols.Length; i++)
            {
                if (!Physics.Linecast(transform.TransformPoint(0, 1.5f, 0), cols[i].transform.TransformPoint(0f, 1f, 0), lockOnBlockingLayers))
                {
                    //check if distance is smaller than 
                    float distance = Vector3.Distance(transform.position, cols[i].transform.position);
                    if (distance < maxDistance)
                    {
                        float angle = Vector3.Angle(transform.forward, cols[i].transform.position - transform.position);

                        //Check if angle is small enough
                        if (angle < maxFreeAimShootingAngle)
                        {
                            target = cols[i].transform;
                            maxDistance = distance;
                        }
                    }
                }
            }

            //Set the free aiming target
            currentFreeAimingTarget = target;
        }
    }
}
