using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatfrorm : MonoBehaviour {

    [SerializeField]
    bool PlayerActivation;

    [SerializeField]
    bool PassiveLooping;

    [SerializeField]
    bool ActiveLooping;

    [SerializeField]
    bool ResetIfFall;

    [SerializeField]
    float speed;

    [SerializeField]
    Vector3[] MovingPoints;

    Vector3 Target;

    Coroutine Liike;


	// Use this for initialization
	void Start () {
        for(int i = 0; i < MovingPoints.Length; i++)
        {
            MovingPoints[i] = transform.TransformPoint(MovingPoints[i]);
        }
        if (PassiveLooping)
        {
            ResetIfFall = false;
            PlayerActivation = false;
            ActiveLooping = false;
            Liike = StartCoroutine(StartMove());
        }
        if (ActiveLooping)
        {
            PassiveLooping = false;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for(int i=0; i < MovingPoints.Length-1; i++)
        {
            Gizmos.DrawLine(transform.TransformPoint(MovingPoints[i]), transform.TransformPoint( MovingPoints[i+1]));

            if(i == MovingPoints.Length-2)
            {
                Gizmos.DrawLine(transform.TransformPoint(MovingPoints[i+1]), transform.TransformPoint( MovingPoints[0]));
            }
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(gameObject.transform);     
        if (PlayerActivation)
        {
            Liike = StartCoroutine(StartMove());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
        if (ResetIfFall && ActiveLooping)
        {
            ActiveLooping = false;
        }
        if (!ResetIfFall && !PassiveLooping)
        {
            StopCoroutine(Liike);
        }
    }
    IEnumerator StartMove()
    {
        for (int i = 0; i < MovingPoints.Length; i++)
        {
            if (i < MovingPoints.Length - 1)
            {
                Target = MovingPoints[i + 1];
            }
            if (i == MovingPoints.Length-1)
            {
                Target = MovingPoints[0];
            }


            while (transform.position != Target)
            {
                transform.position = Vector3.MoveTowards(transform.position, Target, Time.deltaTime * speed);
                yield return null;
            }
            if (Target == MovingPoints[0] && ActiveLooping)
            {
                i = -1;
            }
            if (Target == MovingPoints[0] && PassiveLooping)
            {
                i = -1;
            }
        }
    }
    IEnumerator Reset()
    {
        Target = MovingPoints[0];
        while (transform.position != Target)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target, Time.deltaTime * speed);
            yield return null;
        }
    }
}
