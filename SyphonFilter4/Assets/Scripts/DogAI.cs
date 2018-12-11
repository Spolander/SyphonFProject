using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAI : MonoBehaviour {

    public Animator anim;
    public float playerDistance;

    private GameObject player;
    private float biteDistance = 4f;
    private float dogSpeed = 10f;
    private float sleepingDistance = 40f;
    private bool dogSenses = false;
    private RaycastHit hit;

	// Use this for initialization
	void Start () {
        anim.SetBool("Run", false);
        anim.SetBool("Attack", false);
        player = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        CanDogSee();

        playerDistance = Vector3.Distance(this.transform.position, player.transform.position);
        if (playerDistance <= sleepingDistance && dogSenses)
        {
            getCloser();
        }
	}

    void getCloser()
    {
        transform.LookAt(player.transform);
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0);

        //Dog comes closer
        if (playerDistance >= biteDistance)
        {
            this.transform.position += this.transform.forward * dogSpeed * Time.deltaTime;
            anim.SetBool("Attack", false);
        }

        //Dog stops and start attacking
        else if (playerDistance <= biteDistance)
        {
            GameObject.Find("CoolerPlayer").GetComponent<PlayerHealth>().takeDamage(5, player);
            anim.SetBool("Attack", true);
        }
    }

    void CanDogSee()
    {
        if (Physics.Raycast(this.transform.position, (player.transform.position - this.transform.position), out hit, sleepingDistance))
        {
            //Debug.Log("dogSenses: " + dogSenses + " \thit.transform: " + hit.transform);
            if (hit.transform == player.transform)
            {
                anim.SetBool("Run", true);
                dogSenses = true;
            }
            else
            {
                dogSenses = false;
                anim.SetBool("Run", false);
            }
        }
    }
}
