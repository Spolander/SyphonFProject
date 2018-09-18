using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySplitDoor : MonoBehaviour {
    [SerializeField]
    private bool Status;                    //door status: false=closed, true=open
    [SerializeField]
    private float doorSpeed = 2.0f;

    GameObject doorL;
    GameObject doorR;
    Vector3 doorLClosed;
    Vector3 doorRClosed;
    Vector3 doorLOpen;
    Vector3 doorROpen;

    Vector3 Right = Vector3.right;
    Vector3 Left = Vector3.left;

    bool opening;
    bool closing;

    // Use this for initialization
    void Start () {
        Status = false;
        doorL = transform.Find("DoorL").gameObject;
        doorR = transform.Find("DoorR").gameObject;
        doorLClosed = doorL.transform.position;
        doorRClosed = doorR.transform.position;
        doorLOpen = doorLClosed - new Vector3(0, 30, 0);
        doorROpen = doorRClosed - new Vector3(0, -30, 0);
    }
	
	// Update is called once per frame
	void Update () {

	}
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" && Status == false)
        {
            Debug.Log("Opening");
            if (!opening)
            {
                StartCoroutine(Open());

            }

        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" && Status == false)
        {
            Debug.Log("Closing");
            closing = true;


            StartCoroutine(Close());
        }
    }
    IEnumerator Open()
    {
        opening = true;
        closing = false;
        yield return null;
    }
    IEnumerator Close()
    {
        opening = false;
        closing = true;
        yield return null;
    }
}
