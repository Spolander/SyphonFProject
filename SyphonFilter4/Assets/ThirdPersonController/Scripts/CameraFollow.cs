using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Transform lockOntarget;
    public Transform LockOnTarget { get { return lockOntarget; } set { lockOntarget = value; } }

    private Transform player;

    public float defaultDistance;
    public float lockOnDistance;
    public float height = 2;

    [SerializeField]
    private float defaultLookAtHeight = 1;
    [SerializeField]
    private float lockOnLookAtHeight = 1;

    public float moveSpeed = 300;
    public float rotateSpeed = 300;

    private float rotationAngleY;
    public float RotationAngleY { set { rotationAngleY = value; } }

    [SerializeField]
    private float sensitivityX = 100;


    public static CameraFollow playerCam;
	// Use this for initialization

    private void Awake()
    {
        playerCam = this;
        rotationAngleY = transform.eulerAngles.y;
    }
    private void Start()
    {
        player = PlayerCharacterController.player.transform;
    }
    // Update is called once per frame
    void LateUpdate () {
        if (player)
        {
            rotationAngleY += Time.deltaTime * Input.GetAxisRaw("Mouse X")*sensitivityX;

            if (lockOntarget)
            {
                Vector3 dir = lockOntarget.position - player.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((lockOntarget.position + Vector3.up * lockOnLookAtHeight) - transform.position), rotateSpeed * Time.deltaTime);


                Vector3 translation = Vector3.Scale(dir.normalized, new Vector3(1, 0, 1));
                translation.Normalize();
                transform.position = Vector3.MoveTowards(transform.position, (player.position + translation * -lockOnDistance) + Vector3.up * height, moveSpeed * Time.deltaTime);
            }
            else
            {
                Quaternion rotation = Quaternion.Euler(0, rotationAngleY, 0);
                transform.position = player.position + rotation * Vector3.forward * defaultDistance + Vector3.up*height;
                transform.rotation = Quaternion.LookRotation((player.position + Vector3.up * defaultLookAtHeight) - transform.position);
            }
        }
		
	}
}
