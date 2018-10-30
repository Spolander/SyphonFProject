﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySplitDoor : MonoBehaviour {
    private int inside =0;                  //variable to keep count how many characters are inside trigger
    [SerializeField]
    private float doorSpeed = 0.002f;       //opening & closing speed

    [SerializeField]
    bool UsesKey;
    [SerializeField]
    GameObject KeyEnemy;

    GameObject doorL;
    GameObject doorR;

    Vector3 doorLClosed;                    //initial position of closed doors
    Vector3 doorRClosed;
    Vector3 doorLOpen;                      //position of doors when fully open
    Vector3 doorROpen;

    bool opening;
    bool closing;

    Coroutine Rutiini;                      //make coroutine variable, so we can stop it

    // Use this for initialization
    void Start () {
        doorL = transform.Find("DoorL").gameObject;
        doorR = transform.Find("DoorR").gameObject;

        doorLClosed = doorL.transform.position;                 //set positions where doors are closed
        doorRClosed = doorR.transform.position; 
        
        doorLOpen = doorLClosed - transform.TransformDirection( new Vector3(-2.5f, 0, 0));      //set positions where door is open
        doorROpen = doorRClosed - transform.TransformDirection( new Vector3( 2.5f, 0, 0));
    }
	
	// Update is called once per frame
	void Update () {

	}
    void OnTriggerEnter(Collider collision)
    {
        if (!UsesKey)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
            {
                inside++;
                Debug.Log("Opening");
                if (!opening)
                {
                    if (closing)                    //if  doors are closing, stop closing and start opening
                    {
                        StopCoroutine(Rutiini);
                    }
                    Rutiini = StartCoroutine(Open());
                    SoundEngine.instance.PlaySound("Door", gameObject.transform.position, gameObject.transform);
                }
            }
        }
        else if (UsesKey && KeyEnemy == null)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
            {
                inside++;
                Debug.Log("Opening");
                if (!opening)
                {
                    if (closing)                    //if  doors are closing, stop closing and start opening
                    {
                        StopCoroutine(Rutiini);
                    }
                    Rutiini = StartCoroutine(Open());
                    SoundEngine.instance.PlaySound("Door", gameObject.transform.position, gameObject.transform);
                }
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
            {
                inside--;
                Debug.Log("Closing");
                if (!closing && inside == 0)                //check that the count of characters inside trigger is 0
                {
                    if (opening)                            //if doors are opening, stop opening and start closing
                    {
                        StopCoroutine(Rutiini);
                    }
                    Rutiini = StartCoroutine(Close());
                    SoundEngine.instance.PlaySound("Door", gameObject.transform.position, gameObject.transform);
                }
            }
    }
    private void OnTriggerStay(Collider other)
    {
        
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
