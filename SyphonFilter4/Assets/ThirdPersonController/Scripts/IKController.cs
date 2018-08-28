using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour {

    Transform target;
    public Transform Target { set { target = value; } get { return target; } }


    Vector3 lookPoint;


    Animator anim;

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

    private float currentLookAtWeight = 0;
    public Transform testTarget;

    Transform chest;

    [SerializeField]
    private float aimingDistance = 0.6f;

    
    private Vector3 hintPosition = new Vector3(0.8f, 0.57f, 0);


    [SerializeField]
    private float aimingSpread = 0.39f;

    [SerializeField]
    private float baseAimingHeight = 1;

    Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        chest = anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (target)
            {
                target = null;
                anim.SetBool("lockon", false);
                CameraFollow.playerCam.RotationAngleY = mainCam.transform.eulerAngles.y-180;
                CameraFollow.playerCam.LockOnTarget = null;
            }
               
            else
            {
                anim.SetBool("lockon", true);
                target = testTarget;
                CameraFollow.playerCam.LockOnTarget = target;
            }
              
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {

        Vector3 aimingPoint = transform.TransformPoint(0f, 2, 10);
        if (layerIndex == 1)
        {
            if (target)
            {
                aimingPoint = target.position;
                lookPoint = target.position;
                float y = lookPoint.y;

                lookPoint = transform.position + (lookPoint - transform.position).normalized * aimingDistance*Mathf.Lerp(0.6f, 2, Mathf.Abs(anim.GetFloat("Forward")));
                lookPoint.y = y;
                currentLookAtWeight = Mathf.MoveTowards(currentLookAtWeight, lookAtWeight, Time.deltaTime*3);

            }
            else
            {
                currentLookAtWeight = Mathf.MoveTowards(currentLookAtWeight, 0, Time.deltaTime * 5);
                lookPoint = transform.TransformPoint(0f, 2, 10);
            }
               



            anim.SetLookAtPosition(aimingPoint);
            anim.SetLookAtWeight(currentLookAtWeight, bodyWeight, headWeight, 0, clampWeight);

            anim.SetIKPosition(AvatarIKGoal.RightHand, lookPoint+mainCam.transform.right*aimingSpread);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, currentLookAtWeight*handPositionWeight);

            anim.SetIKPosition(AvatarIKGoal.LeftHand, lookPoint + mainCam.transform.right * -aimingSpread);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, currentLookAtWeight*handPositionWeight);

            Quaternion look = Quaternion.LookRotation(aimingPoint-transform.TransformPoint(0,baseAimingHeight,0));
            anim.SetIKRotation(AvatarIKGoal.RightHand, look*Quaternion.AngleAxis(-90,Vector3.forward));
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, currentLookAtWeight);

            anim.SetIKRotation(AvatarIKGoal.LeftHand, look * Quaternion.AngleAxis(90, Vector3.forward));
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, currentLookAtWeight);

            anim.SetIKHintPosition(AvatarIKHint.RightElbow, transform.TransformPoint(hintPosition.x, hintPosition.y, 0));
            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, currentLookAtWeight);
        }
        
    }

    private void LateUpdate()
    {
      //  chest.transform.eulerAngles = new Vector3(chest.transform.eulerAngles.x, ClampAngle(chest.transform.eulerAngles.y, transform.eulerAngles.y-90, transform.eulerAngles.y+90f), chest.transform.eulerAngles.z);
    }


}
