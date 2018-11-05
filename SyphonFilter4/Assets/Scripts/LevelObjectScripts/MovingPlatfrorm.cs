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
    bool UseAsElevator;

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
        if (UseAsElevator)
        {
            ActiveLooping = false;
            PassiveLooping = false;
            ResetIfFall = false;
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
        
        if (PlayerActivation && Liike ==null)
        {
            Liike = StartCoroutine(StartMove());
            Debug.Log(speed);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!UseAsElevator)
        {
            other.transform.SetParent(null);
            other.transform.localScale = Vector3.one;
            if (ResetIfFall && ActiveLooping)
            {
                ActiveLooping = false;
            }
            if (!ResetIfFall && !PassiveLooping)
            {
                StopCoroutine(Liike);
                SoundEngine.instance.StopSound("MovingPlatformSound" , transform);
            }
        }
        else if (UseAsElevator)
        {
            other.transform.SetParent(null);
            other.transform.localScale = Vector3.one;
        }
    }
    IEnumerator StartMove()
    {
        if (!UseAsElevator)
        {
            SoundEngine.instance.PlaySound("MovingPlatformSound", gameObject.transform.position, transform);
            for (int i = 0; i < MovingPoints.Length; i++)
            {
                if (i < MovingPoints.Length - 1)
                {
                    Target = MovingPoints[i + 1];
                }
                if (i == MovingPoints.Length - 1)
                {
                    Target = MovingPoints[0];
                }


                while (transform.position != Target)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Target, Time.deltaTime * speed);
                    yield return null;
                }
                if (transform.position == MovingPoints[0] && !PassiveLooping && !ActiveLooping)
                {
                    SoundEngine.instance.StopSound("MovingPlatformSound", transform);
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
        else if (UseAsElevator)
        {
            
            if (Target == Vector3.zero)
            {
                Target = MovingPoints[0];
                Debug.Log("movingpoint0");
            }
            SoundEngine.instance.PlaySound("MovingPlatformSound", gameObject.transform.position, transform);
            if (Target == MovingPoints[0])
            {
                Target = MovingPoints[1];
                Debug.Log("movingpoint1");
            }
            else if (Target==MovingPoints[1])
            {
                Target = MovingPoints[0];
                Debug.Log("movingpoint0");
            }
            while (transform.position != Target)
            {
                transform.position = Vector3.MoveTowards(transform.position, Target, Time.deltaTime * speed);
                yield return null;
            }
          
            if (transform.position == Target)
            {
                SoundEngine.instance.StopSound("MovingPlatformSound", transform);
            }
        }
        Liike = null;
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
