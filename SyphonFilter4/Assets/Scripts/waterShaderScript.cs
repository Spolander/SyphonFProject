using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterShaderScript : MonoBehaviour {


    public float limit;
    public float interval;
    public float speedX;
    public float speedY;
    public float speedVariance;
    float x;
    float y;

    float targetX;
    float targetY;
    Material m;
    float timer;

    float _speedX;
    float _speedY;
	// Use this for initialization
	void Start () {
        m = GetComponent<Renderer>().material;

        targetX = limit;
        targetY = -limit;

        _speedX = speedX + Random.Range(-speedVariance, speedVariance);
        _speedY = speedY + Random.Range(-speedVariance, speedVariance);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        x = Mathf.MoveTowards(x, targetX, Time.deltaTime * _speedX);
        y = Mathf.MoveTowards(y, targetY, Time.deltaTime * _speedY);


        if (Mathf.Approximately(x,targetX))
        {
            timer = 0;
            targetX *= -1;
            targetY *= -1;

            _speedX = speedX + Random.Range(-speedVariance, speedVariance);
            _speedY = speedY + Random.Range(-speedVariance, speedVariance);
        }

        m.SetFloat("_RefractionX", x);
        m.SetFloat("_RefractionY", y);
    }
}
