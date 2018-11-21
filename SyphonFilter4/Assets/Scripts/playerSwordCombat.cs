using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSwordCombat : MonoBehaviour {

    Animator anim;

    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update () {

        if (Input.GetKeyDown(KeyCode.Mouse0))
            anim.SetTrigger("swordHit");
	}
}
