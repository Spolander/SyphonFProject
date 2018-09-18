using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySplitDoor : MonoBehaviour {
    [SerializeField]
    private bool Status;                    //door status: false=closed, true=open
    [SerializeField]
    private float doorSpeed = 0.002f;       //opening & closing speed

    GameObject doorL;
    GameObject doorR;

    Vector3 doorLClosed;                    //initial position of closed doors
    Vector3 doorRClosed;
    Vector3 doorLOpen;                      //position of doors when fully open
    Vector3 doorROpen;

    bool opening;
    bool closing;

    Coroutine Rutiini;

    // Use this for initialization
    void Start () {
        Status = false;
        doorL = transform.Find("DoorL").gameObject;
        doorR = transform.Find("DoorR").gameObject;
        doorLClosed = doorL.transform.position;
        doorRClosed = doorR.transform.position;
        doorLOpen = doorLClosed - new Vector3(-2.5f, 0, 0);
        doorROpen = doorRClosed - new Vector3(2.5f, 0, 0);
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
                if (closing)
                {
                    StopCoroutine(Rutiini);
                }
                Rutiini = StartCoroutine(Open());
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" && Status == false)
        {
            Debug.Log("Closing");
            if (!closing)
            {
                if (opening)
                {
                    StopCoroutine(Rutiini);
                }
                Rutiini = StartCoroutine(Close());
            }
        }
    }
    IEnumerator Open()
    {
        opening = true;
        closing = false;
        while (doorL.transform.position != doorLOpen && doorR.transform.position != doorROpen)
        {
            doorL.transform.position = Vector3.MoveTowards(doorL.transform.position, doorLOpen, Time.deltaTime * doorSpeed);
            doorR.transform.position = Vector3.MoveTowards(doorR.transform.position, doorROpen, Time.deltaTime * doorSpeed);
            yield return null;
        }      
    }
    IEnumerator Close()
    {
        opening = false;
        closing = true;
        while (doorL.transform.position != doorLClosed && doorR.transform.position != doorRClosed)
        {
            doorL.transform.position = Vector3.MoveTowards(doorL.transform.position, doorLClosed, Time.deltaTime * doorSpeed);
            doorR.transform.position = Vector3.MoveTowards(doorR.transform.position, doorRClosed, Time.deltaTime * doorSpeed);
            yield return null;
        } 
        yield return null;
    }
}
