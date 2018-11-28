using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour {

    [SerializeField]
    protected float Health = 100;

    [SerializeField]
    private Transform TargetPoint;

    //returns targetPoint if it's not null, else return position + up
    public Vector3 centerPoint { get { if (TargetPoint) return TargetPoint.position; else return transform.position+Vector3.up;} }

    public virtual void takeDamage(float amount, GameObject caller) {
        Health = Health - amount;
        Debug.Log(Health);
        if (Health <= 0)
        {
            death(caller);
        }
    }

    public virtual void death(GameObject caller)
        {
            
     
        Destroy(gameObject);
    }

    public virtual void heal(float amount)
    {
        Health = Health + amount;
    }
}
