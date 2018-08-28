using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour {


    CharacterController controller;
    Camera mainCam;

    Vector3 moveVector;

    [SerializeField]
    private float airMoveSpeed = 6;

    [SerializeField]
    private float moveSpeed = 6;

 

    float gravityTarget = 20;
    float gravity = 0;

    bool canControl = true;

    Animator anim;

    [SerializeField]
    private float rotateSpeed = 300;

    IKController ikc;

	// Use this for initialization
	void Start () {
        ikc = GetComponent<IKController>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

        if (canControl)
            Move();
	}

    private void Move()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        moveVector = mainCam.transform.right * inputVector.x + camForward * inputVector.y;
        moveVector.y = gravity;

       
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z, 0.05f, Time.deltaTime);
            anim.SetFloat("Horizontal", transform.InverseTransformDirection(moveVector).x);

       
        if (inputVector.magnitude > 0.1f)
        {
            if (ikc.Target == null)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.Scale(new Vector3(1, 0, 1), moveVector)), Time.deltaTime * rotateSpeed);
            else
            {
                Vector3 dir = ikc.Target.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            }
          
        }
        else
        {
            if (ikc.Target)
            {
                Vector3 dir = ikc.Target.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            }
               

        }

        Vector3 v = moveVector*Time.deltaTime*moveSpeed;
        v.y = gravity * Time.deltaTime;

        controller.Move(v);

    }

    private void OnAnimatorMove()
    {
        Vector3 v = anim.deltaPosition;
      
    }
}
