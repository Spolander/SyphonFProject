using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPickUp : MonoBehaviour {

    [SerializeField]
    int JumpPower = 20;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 100, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerCharacterController>().setJumpForce(JumpPower);
            Destroy(gameObject);
        }
    }
}
