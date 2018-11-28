using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : BaseHealth {

    [SerializeField]
    protected GameObject bloodParticle;

    [SerializeField]
    protected GameObject cogParticle;

    [SerializeField]
    protected float chanceToSpawnBlood= 0.8f;

    [SerializeField]
    protected float chanceToSpawnCogs = 0.5f;
    public override void takeDamage(float amount, GameObject caller)
    {
        Health = Health - amount;
        Debug.Log(Health);

        if (bloodParticle && Random.value < chanceToSpawnBlood)
        {
            GameObject g = (GameObject)Instantiate(bloodParticle, transform.position + Vector3.up, transform.rotation * Quaternion.AngleAxis(Random.Range(-45f, 45), Vector3.up));
            g.transform.SetParent(transform);
        }

        if (cogParticle && Random.value < chanceToSpawnCogs)
        {
            GameObject g = (GameObject)Instantiate(cogParticle, transform.position + Vector3.up, transform.rotation * Quaternion.AngleAxis(Random.Range(-45f, 45), Vector3.up));
            g.transform.SetParent(transform);
        }
        if (Health <= 0)
        {
            death(caller);
        }
        else
        {
            if(GetComponent<BaseAI>())
            GetComponent<BaseAI>().LastAggroTime = Time.time;
        }
    }
}
