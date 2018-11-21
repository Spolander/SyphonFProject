using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{


    CharacterController controller;
    Camera mainCam;

    Vector3 moveVector;

    [SerializeField]
    private float airMoveSpeed = 6;

    [SerializeField]
    private float moveSpeed = 6;

    public bool IsInWater;

    float gravityTarget = 30;
    public float gravity = 0;
    [SerializeField]
    private float gravityAcceleration = 30;

    bool canControl = true;

    Animator anim;

    [SerializeField]
    private float rotateSpeed = 300;




    Vector3 groundNormal = Vector3.up;
    Vector3 slopeNormal = Vector3.up;

    private bool isJumping = false;
    private bool isLedgeJumping = false;

    [SerializeField]
    private LayerMask whatIsGround;


    [SerializeField]
    private float jumpingForce = 20;

    [SerializeField]
    private float ledgeJumpForce = 10;

    private float initialJumpingForce;

    [SerializeField]
    private float jumpingDuration = 0.2f;



    private float lastJumpTime;

    public static PlayerCharacterController player;

    private bool grounded = true;
    private float lastGroundedTime;

    private bool dashing = false;
    public bool Dashing { set { dashing = value; } get { return dashing; } }

    private bool airDashing = false;
    public bool AirDashing { set { airDashing = value; } }

    [SerializeField]
    private float dashSpeedMultiplier = 1;

    [SerializeField]
    private float airDashSpeedMultiplier = 1.2f;

    //how long before falling is registered
    private float groundedLossTime = 0.1f;

    //has the player jumped and not just fallen off
    private bool hasJumped = false;

    private bool hasLedgeJumped = false;

    Vector3 animMatchingPoint;
    Quaternion animMatchingRotation;

    private bool hangingFromLedge = false;
    public bool HangingFromLedge { get { return hangingFromLedge; } }
    private float ledgeJumpTime;

    [SerializeField]
    private float swordRootmotionSpeed = 1;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
        initialJumpingForce =jumpingForce;
    }
    private void Awake()
    {
        player = this;
    }
    // Update is called once per frame
    void Update()
    {
        checkGrounded();
        anim.SetBool("grounded", grounded);


        if (canControl && hangingFromLedge == false)
            Move();


        if (!grounded)
        {
            if (airDashing == false && Input.GetButtonDown("Jump") && (hasJumped || hasLedgeJumped) && hangingFromLedge == false)
                AirDash();
            else if (Input.GetButtonDown("Jump") && hangingFromLedge == true)
                StartCoroutine(jumpingAnimation(true));

            if (hangingFromLedge == false && hasJumped && controller.velocity.y < 0)
                checkLedgeGrab();
        }

        if (isJumping)
        {
            gravity = -jumpingForce;

        }
        else if (isLedgeJumping)
        {
            gravity = -ledgeJumpForce;
        }
        else
        {
            if (grounded)
            {
                gravity = 1;

                if (isJumping == false && Input.GetKeyDown(KeyCode.Space) && hangingFromLedge == false)
                    StartCoroutine(jumpingAnimation(false));
                else if (isLedgeJumping == false && Input.GetKeyDown(KeyCode.Space) && hangingFromLedge)
                    StartCoroutine(jumpingAnimation(true));

                if (isJumping == false && dashing == false && Input.GetKeyDown(KeyCode.LeftShift) && anim.IsInTransition(0) == false)
                {
                    Dash();
                }
            }
            else if (isJumping == false)
            {
                gravity = Mathf.MoveTowards(gravity, gravityTarget, Time.deltaTime * gravityAcceleration);
            }
        }

        MatchAnimator();

    }
    private void Dash()
    {
        dashing = true;
        anim.CrossFadeInFixedTime("Slide", 0);
       
    }

    private void AirDash()
    {
        airDashing = true;
        anim.CrossFadeInFixedTime("AirDash", 0.15f);
    }
    IEnumerator jumpingAnimation(bool ledgeJump)
    {
        if (ledgeJump)
        {
            hasLedgeJumped = true;
            isLedgeJumping = true;
            ledgeJumpTime = Time.time;
            hangingFromLedge = false;
            controller.enabled = true;
            canControl = true;
            anim.CrossFadeInFixedTime("LedgeJump", 0.1f);
        }
        else
        {
            hasJumped = true;
            anim.CrossFadeInFixedTime("jump", 0.1f);
            isJumping = true;
        }

            

      
        groundNormal = Vector3.up;
        grounded = false;
        lastJumpTime = Time.time;
        anim.SetBool("grounded", false);

        yield return new WaitForSeconds(jumpingDuration);

        isJumping = false;
        isLedgeJumping = false;
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




        if(moveVector.magnitude > 0.1f)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.Scale(new Vector3(1, 0, 1), moveVector)), Time.deltaTime * rotateSpeed);
        

        Vector3 v = moveVector * moveSpeed;
      
        // v.y = gravity * -1;

        if (slopeNormal.y <= 0.7f)
        {
            v.y = gravity * -1;
            v = Vector3.ProjectOnPlane(v, -slopeNormal);

            if (moveVector.magnitude > 0.15)
                v = v.normalized * moveVector.magnitude * moveSpeed;

        }
        else
        {
            float magnitude = moveVector.magnitude;
            v = Vector3.ProjectOnPlane(v, groundNormal);

            v = v.normalized * magnitude * moveSpeed;
            if (dashing)
            {
                if (moveVector.magnitude < 1)
                    v = transform.forward * moveSpeed;
                v *= dashSpeedMultiplier;
            }
            else if (airDashing)
            {
                if (moveVector.magnitude < 1)
                    v = transform.forward * moveSpeed;
                v *= airDashSpeedMultiplier;
            }
            v.y -= gravity;
        }


        //check if we are attacking currently 
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("swordhit") && anim.IsInTransition(0) == false)
        {
            Vector3 rootmotion = transform.forward * swordRootmotionSpeed * anim.GetFloat("ForwardRootMotion");
            v.x = rootmotion.x;
            v.z = rootmotion.z;
        }

        controller.Move(v * Time.deltaTime);


    }

    private void OnAnimatorMove()
    {
        if (hangingFromLedge)
        {
            transform.position = anim.rootPosition;
            transform.rotation = anim.rootRotation;
        }
           

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //print(hit.collider.name);
    }


    void checkGrounded()
    {

        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, controller.radius + 0.05f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, 1.5f, 0f), Vector3.down);
        Ray wallRay = new Ray(transform.TransformPoint(0, 1, 0), transform.forward);


        if (Physics.SphereCast(sphereRay, controller.radius, out hit, 1.5f, whatIsGround, QueryTriggerInteraction.Ignore) && !grounded)
        {
            slopeNormal = hit.normal;

        }
        else if (Physics.Raycast(ray, out hit, 1f, whatIsGround, QueryTriggerInteraction.Ignore) && !grounded)
        {
            slopeNormal = hit.normal;
        }
        else
            slopeNormal = Vector3.up;

        if (Time.time > lastJumpTime + 0.2f)
        {
            if (Physics.SphereCast(sphereRay, controller.radius, out hit, 1.5f, whatIsGround, QueryTriggerInteraction.Ignore))
             {
                if (hit.normal.y > 0.7f)
                {
                    groundNormal = hit.normal;
                    grounded = true;
                    lastGroundedTime = Time.time;
                    hasJumped = false;
                    hasLedgeJumped = false;
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
    public void setJumpForce(int amount)
    {
        jumpingForce = amount;
    }
    public void resetJumpForce()
    {
        jumpingForce = initialJumpingForce;
    }

    void checkLedgeGrab()
    {
        if (Time.time < ledgeJumpTime + 1)
            return;

        Ray ray = new Ray(transform.TransformPoint(0f, 1f * transform.localScale.x, 0f), transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f, whatIsGround))
        {
            if (Vector3.Angle(-hit.normal, transform.forward) < 60f)
            {
                Vector3 downPoint = hit.point + (-hit.normal) * 0.5f + Vector3.up * 1.5f;
                Ray forwardRay = new Ray(new Vector3(transform.position.x, downPoint.y, transform.position.z), transform.forward);
                RaycastHit forwardHit;
                if (Physics.Raycast(forwardRay, out forwardHit, 1f, whatIsGround, QueryTriggerInteraction.Ignore))
                    return;

                Ray downRay = new Ray(downPoint, Vector3.down);
                RaycastHit downHit;
                if (Physics.Raycast(downRay, out downHit, 2f, whatIsGround))
                {
                    Vector3 dir = -hit.normal;
                    dir.y = 0f;
                    animMatchingPoint = new Vector3(hit.point.x, downHit.point.y-2.65f, hit.point.z) + hit.normal * 0.3f;
                    Vector3 lookDirection = -hit.normal;
                    lookDirection.y = 0;
                    animMatchingRotation = Quaternion.LookRotation(lookDirection.normalized);
                    hangingFromLedge = true;
                    anim.Play("LedgeGrab");
              
                    controller.enabled = false;
                }
            }
        }
    }

    void MatchAnimator()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab") && anim.IsInTransition(0) == false)
        {
            anim.MatchTarget(animMatchingPoint, animMatchingRotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1), 0, 0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(animMatchingPoint, 0.1f);
    }
}
