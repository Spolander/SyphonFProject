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

 

    float gravityTarget = 30;
    public float gravity = 0;
    [SerializeField]
    private float gravityAcceleration = 30;

    bool canControl = true;

    Animator anim;

    [SerializeField]
    private float rotateSpeed = 300;

    IKController ikc;


    Vector3 groundNormal = Vector3.up;
    Vector3 slopeNormal = Vector3.up;

    private bool isJumping = false;

    [SerializeField]
    private LayerMask whatIsGround;


    [SerializeField]
    private float jumpingForce = 20;

    [SerializeField]
    private float jumpingDuration = 0.2f;
 


    private float lastJumpTime;

    public static PlayerCharacterController player;

    private bool grounded = true;
    private float lastGroundedTime;

    //how long before falling is registered
    private float groundedLossTime = 0.1f;
	// Use this for initialization
	void Start () {
        ikc = GetComponent<IKController>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
	}
    private void Awake()
    {
        player = this;
    }
    // Update is called once per frame
    void Update () {



        checkGrounded();
        anim.SetBool("grounded", grounded);
        if (canControl)
            Move();


        if (isJumping)
        {
            gravity = -jumpingForce;
        }
        else
        {
            if (grounded)
            {
                gravity = 1;

                if (isJumping == false && Input.GetKeyDown(KeyCode.Space))
                    StartCoroutine(jumpingAnimation());
            }
            else if (isJumping == false)
            {
                gravity = Mathf.MoveTowards(gravity, gravityTarget, Time.deltaTime * gravityAcceleration);
            }
        }
       
       
	}

    IEnumerator jumpingAnimation()
    {
        grounded = false;
        lastJumpTime = Time.time;
        anim.SetBool("grounded", false);
        isJumping = true;
        anim.CrossFadeInFixedTime("jump", 0.1f);
        yield return new WaitForSeconds(jumpingDuration);
        isJumping = false;
    }

    private void Move()
    {

        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (inputVector.magnitude > 1)
            inputVector.Normalize();

        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        moveVector = mainCam.transform.right * inputVector.x + camForward * inputVector.y;
        
        

        anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z, 0.05f, Time.deltaTime);
        anim.SetFloat("Horizontal", transform.InverseTransformDirection(moveVector).x);

        if (moveVector.magnitude > 1)
            moveVector.Normalize();

         

       
        if (inputVector.magnitude > 0.1f)
        {
            if (ikc.LockedOn == false)
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
            if (ikc.LockedOn)
            {
                Vector3 dir = ikc.Target.position - transform.position;
                dir.y = 0;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            }
               

        }

        Vector3 v = moveVector * moveSpeed;
        v.y = gravity * -1;
      
            if (slopeNormal.y <= 0.7f)
            {
                v = -Vector3.up * gravity;
                v = Vector3.ProjectOnPlane(v, -slopeNormal);
            }
            else
            {
               
                v = Vector3.ProjectOnPlane(v, groundNormal);
                v = v.normalized * moveVector.magnitude * moveSpeed;
            }
        


        if (!grounded || groundNormal == Vector3.up)
            v.y = gravity * -1;

        Debug.DrawRay(transform.TransformPoint(0, 1, 0), v,Color.red);



       
        
        controller.Move(v*Time.deltaTime);
       // print(controller.velocity.magnitude);


    }

    private void OnAnimatorMove()
    {
        
    }


   

    void checkGrounded()
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, controller.radius + 0.05f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, 1.5f, 0f), Vector3.down);
        Ray wallRay = new Ray(transform.TransformPoint(0, 1, 0), transform.forward);

        if (Physics.SphereCast(sphereRay, controller.radius, out hit, 1.5f, whatIsGround) && !grounded)
        {
            slopeNormal = hit.normal;

        }
        else if (Physics.Raycast(ray, out hit, 0.2f, whatIsGround) && !grounded)
        {
            slopeNormal = hit.normal;
        }
        else
            slopeNormal = Vector3.up;

        if (Time.time > lastJumpTime + 0.2f)
        {
            if (Physics.Raycast(ray, out hit, 1f, whatIsGround))
            {
                if (hit.normal.y > 0.7f)
                {
                    grounded = true;
                    lastGroundedTime = Time.time;
                    return;
                }
                else if (Time.time > lastGroundedTime + groundedLossTime)
                {
                    grounded = false;
                    return;
                }
            }
        }
        if (Time.time > lastGroundedTime + groundedLossTime)
        {
            grounded = false;
        }

    }
}
