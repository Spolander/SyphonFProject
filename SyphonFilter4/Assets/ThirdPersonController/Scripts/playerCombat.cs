using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCombat : MonoBehaviour {

    IKController ikc;
    Animator anim;
    Camera mainCam;

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

    private Transform currentFreeAimingTarget = null;

    private MuzzleFlashAnimation muzzles;
	void Start () {
        muzzles = GetComponent<MuzzleFlashAnimation>();
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        ikc = GetComponent<IKController>();
	}
	
	// Update is called once per frame
	void Update () {

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


        ikc.Shooting = Input.GetKey(KeyCode.Mouse0);

        if (ikc.Shooting)
        {
            if (ikc.LockedOn)
            {
                //shoot towards target
            }
            else
            {
                scanForFreeAim();

                if (currentFreeAimingTarget)
                {
                    ikc.Target = currentFreeAimingTarget;
                }
                else
                {
                    ikc.Target = null;
                }
            }

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

    void stopLockOn()
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

            //Set the free aiming target
            currentFreeAimingTarget = target;
        }
    }
}
