using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSwordCombat : MonoBehaviour {

    Animator anim;

    [SerializeField]
    private Transform sword;

    [SerializeField]
    private Transform hand;

    [SerializeField]
    private Transform spine;

    private Vector3 spinePosition = new Vector3(0.00399f, 0.00789f, -0.00075f);
    private Vector3 spineEuler = new Vector3(-216.654f, -69.93298f, 21.22899f);

    private Vector3 handPosition = new Vector3(0.00077f, 0.00109f, 0.00048f);
    private Vector3 handEuler = new Vector3(-70.25201f, -12.862f, 99.54201f);

    [SerializeField]
    private Vector3 hitboxSize;

    [SerializeField]
    private Vector3 hitBoxLocation;

    [SerializeField]
    private LayerMask hitDetectionLayers;


    // Update is called once per frame

    private void Start()
    {
        anim = GetComponent<Animator>();

        ParentSwordToSpine();
    }

    public void ParentSwordToHand()
    {
        sword.SetParent(hand);
        sword.localPosition = handPosition;
        sword.localEulerAngles = handEuler;
    }

    public void ParentSwordToSpine()
    {
        sword.SetParent(spine);
        sword.localPosition = spinePosition;
        sword.localEulerAngles = spineEuler;

    }


    void Update () {

        if (Input.GetKeyDown(KeyCode.Mouse0))
            anim.SetTrigger("swordHit");
	}

    private void OnDrawGizmos()
    {
        Color c = Color.blue;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawCube(transform.TransformPoint(hitBoxLocation), hitboxSize);
    }
}
