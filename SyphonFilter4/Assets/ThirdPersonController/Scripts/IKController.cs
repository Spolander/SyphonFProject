using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {

    Transform target;
    public Transform Target { set { target = value; } get { return target; } }

    //point for the IK look position
    Vector3 lookPoint;


    Animator anim;

    //multiplier 0-1 for the ik weights
    [SerializeField]
    float lookAtWeight = 1;

    [SerializeField]
    private float handPositionWeight = 0.45f;

    [SerializeField]
    private float bodyWeight = 1;

    [SerializeField]
    private float headWeight = 1;

    [SerializeField]
    private float clampWeight = 1;


    //look at weight that is moved between 0 and lookAtWeight for smooth lookat transitioning
    private float currentLookAtWeight = 0;
    public float CurrentLookAtWeight { get { return currentLookAtWeight; } }

    //how far the hands reach from the player towards the aiming target
    [SerializeField]
    private float aimingDistance = 0.6f;

    
    private Vector3 hintPosition = new Vector3(0.8f, 0.57f, 0);


    [SerializeField]
    private float aimingSpread = 0.39f;


    [SerializeField]
    private float baseAimingHeight = 1;

    [SerializeField]
    private float lookAtPointYOffset = 1.5f;

    [SerializeField]
    private float closeAimingHeight = 1.7f;

    private float closeAimingDistance = 4;


    //aiming height for smoothing aiming height between close and far
    private float currentAimingHeight;


    bool shooting = false;
    public bool Shooting { set { shooting = value; } get { return shooting; } }

    bool lockedOn = false;
    public bool LockedOn { get { if (target == null) return false; else return lockedOn; }set { lockedOn = value; } }
    Camera mainCam;
    private void Start()
    {
        currentAimingHeight = baseAimingHeight;
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //default aiming point is the player forward direction
        Vector3 aimingPoint = transform.TransformPoint(0f, 2, 10);

        if (layerIndex == 1)
        {
            if (target)
            {
                aimingPoint = target.position+Vector3.up*lookAtPointYOffset;
                lookPoint = target.position;
                float y = lookPoint.y;

                lookPoint = Vector3.MoveTowards(lookPoint, transform.position + (lookPoint - transform.position).normalized * Mathf.Lerp(aimingDistance, 2, -anim.GetFloat("Forward")), Time.deltaTime * 35);
                lookPoint.y = y;
                currentLookAtWeight = Mathf.MoveTowards(currentLookAtWeight, lookAtWeight, Time.deltaTime * 3);

            }
            else if (shooting)
            {
                currentLookAtWeight = Mathf.MoveTowards(currentLookAtWeight, lookAtWeight, Time.deltaTime * 5);
                lookPoint = Vector3.MoveTowards(lookPoint, transform.TransformPoint(0f, 2, 10), Time.deltaTime*50);
            }
            else
            {
                currentLookAtWeight = Mathf.MoveTowards(currentLookAtWeight, 0, Time.deltaTime * 5);
                lookPoint = Vector3.MoveTowards(lookPoint, transform.TransformPoint(0f, 2, 10), Time.deltaTime*50);
            }
               



            anim.SetLookAtPosition(aimingPoint);
            anim.SetLookAtWeight(currentLookAtWeight, bodyWeight, headWeight, 0, clampWeight);

            anim.SetIKPosition(AvatarIKGoal.RightHand, lookPoint+mainCam.transform.right*aimingSpread);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, currentLookAtWeight*handPositionWeight);

            anim.SetIKPosition(AvatarIKGoal.LeftHand, lookPoint + mainCam.transform.right * -aimingSpread);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentLookAtWeight*handPositionWeight);

            if (Vector3.Distance(transform.position, aimingPoint) < closeAimingDistance)
            {
                currentAimingHeight = Mathf.MoveTowards(currentAimingHeight, closeAimingHeight, Time.deltaTime * 5);
            }
            else
            {
                currentAimingHeight = Mathf.MoveTowards(currentAimingHeight, baseAimingHeight, Time.deltaTime * 5);
            }

            Quaternion look = Quaternion.LookRotation(aimingPoint-transform.TransformPoint(0,currentAimingHeight,0));
            anim.SetIKRotation(AvatarIKGoal.RightHand, look*Quaternion.AngleAxis(-90,Vector3.forward));
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, currentLookAtWeight);

            anim.SetIKRotation(AvatarIKGoal.LeftHand, look * Quaternion.AngleAxis(90, Vector3.forward));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, currentLookAtWeight);

            anim.SetIKHintPosition(AvatarIKHint.RightElbow, transform.TransformPoint(hintPosition.x, hintPosition.y, 0));
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, currentLookAtWeight);

           
        }
        
    }

  


}
