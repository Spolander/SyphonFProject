using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    [SerializeField]
    protected LayerMask hitDetectionLayers;

    [SerializeField]
    private float damage;

    Transform targetPoint;

    Vector3 targetLocation;
    Vector3 startingPoint;

    private float projectileTime;

    Coroutine COROU;

    bool hit = false;

    public void Initialize(Vector3 targetLocation, float time, float damage)
    {
        //mainCam = Camera.main;
        //spriteTransform = transform.GetChild(0);

        this.damage = damage;
        startingPoint = transform.position;
        this.targetLocation = targetLocation;
        projectileTime = time;

        if (COROU != null)
        {
            StopCoroutine(COROU);
        }
        COROU = StartCoroutine(moveAnimation());
    }
    IEnumerator moveAnimation()
    {
        float lerp = 0;

        while (lerp <= 1 && !hit)
        {
            //spriteTransform.rotation = Quaternion.LookRotation(mainCam.transform.position - transform.position);
            Vector3 pos = Vector3.Lerp(startingPoint, targetLocation, lerp);

            lerp += Time.deltaTime / projectileTime;
            transform.position = pos;
            yield return null;
        }

        Destroy(gameObject);
        SoundEngine.instance.PlaySound("grenade", transform.position, null);
        COROU = null;
        yield return null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collide");
        if (collision.GetComponent<BaseHealth>())
        {
            collision.GetComponent<BaseHealth>().takeDamage(damage, gameObject);
        }
        hit = true; //Set hit to true to exit the coroutine loop
    }
}
