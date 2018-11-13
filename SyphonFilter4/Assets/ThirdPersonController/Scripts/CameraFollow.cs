using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Camera values when the camera is nearest to the player")]
    public float minimumDistance;
    public float minimumHeight = 1;
    [SerializeField]
    private float minimumLookAtHeight = 1;

    [Space]

    public float moveSpeed = 300;
    public float rotateSpeed = 300;

    private float rotationAngleY;
    public float RotationAngleY { set { rotationAngleY = value; } }

    [SerializeField]
    private float sensitivityX = 100;


    public static CameraFollow playerCam;

    [SerializeField]
    private LayerMask cameraBlockingLayers;
	// Use this for initialization

    private void Awake()
    {
        playerCam = this;
       // rotationAngleY = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        player = PlayerCharacterController.player.transform;
    }
    // Update is called once per frame
    void LateUpdate () {

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


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

                Vector3 targetPosition = player.position + rotation * Vector3.forward * defaultDistance + Vector3.up * height;
                RaycastHit hit;

                float lerp = 1;
                if (Physics.Linecast(player.position + Vector3.up, targetPosition, out hit, cameraBlockingLayers, QueryTriggerInteraction.Ignore))
                {
                    lerp = hit.distance / defaultDistance;
                }


                targetPosition = player.position + rotation * Vector3.forward * Mathf.Lerp(minimumDistance, defaultDistance,lerp) + Vector3.up * Mathf.Lerp(minimumHeight, height,lerp);

                transform.position = targetPosition;
                transform.rotation = Quaternion.LookRotation((player.position + Vector3.up * Mathf.Lerp(minimumLookAtHeight, defaultLookAtHeight,lerp)) - transform.position);
            }
        }
		
	}
}
