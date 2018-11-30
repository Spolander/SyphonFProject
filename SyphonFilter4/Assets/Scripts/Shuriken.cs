using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour {


    
    private Vector3 rotation = new Vector3(40, 900, 0);

    [SerializeField]
    private float speed = 1;
    private Vector3 direction;
    [SerializeField]
    private LayerMask collisionLayers;

    [SerializeField]
    private float hitDetectionDistance = 0.5f;

    [SerializeField]
    private float damage = 15;
   


    Vector3 lastPosition;
    // Update is called once per frame


   

    public void Initialize(Vector3 direction)
    {
        this.direction = direction;
        transform.rotation = Quaternion.LookRotation(direction);
        lastPosition = transform.position;
        GetComponentInChildren<TrailRenderer>().enabled = true;

    }
	void Update () {

    
        transform.Rotate(rotation * Time.deltaTime, Space.Self);
        transform.Translate(direction.normalized * Time.deltaTime * speed, Space.World);
        HitDetection();

        if((lastPosition-transform.position).magnitude > 0.5f)
        lastPosition = transform.position;
    }

    void HitDetection()
    {


        RaycastHit hit;

        if (Physics.Linecast(lastPosition, transform.position, out hit,collisionLayers, QueryTriggerInteraction.Ignore))
        {
            transform.rotation = Quaternion.LookRotation(hit.point - transform.position);
            transform.position = hit.point + hit.normal * 0.1f;

            if (hit.collider.GetComponent<enemyHealth>())
                hit.collider.GetComponent<enemyHealth>().takeDamage(damage, gameObject);
            this.enabled = false;
            transform.SetParent(hit.transform);
            StartCoroutine(fadeOut());
        }
    }

    IEnumerator fadeOut()
    {
        yield return new WaitForSecondsRealtime(2);
        Material m = GetComponent<Renderer>().material;

        float lerp = 0;
        while(lerp < 1)
        {
            lerp += Time.deltaTime/2;
            m.SetFloat("_Level", lerp);
            yield return null;
        }

        Destroy(gameObject);
    }
}
