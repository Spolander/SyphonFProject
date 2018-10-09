using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHealth : BaseHealth {

    [SerializeField]
    protected GameObject damageParticleEffect;

    [SerializeField]
    protected float chanceToSpawnEffect = 12;
    public override void takeDamage(float amount, GameObject caller)
    {
        Health = Health - amount;
        Debug.Log(Health);

        if (damageParticleEffect && Random.value < chanceToSpawnEffect)
        {
            GameObject g = (GameObject)Instantiate(damageParticleEffect, transform.position + Vector3.up, transform.rotation * Quaternion.AngleAxis(Random.Range(-45f, 45), Vector3.up));
            g.transform.SetParent(transform);
        }
        if (Health <= 0)
        {
            death(caller);
        }
        else
        {
            GetComponent<BaseAI>().LastAggroTime = Time.time;
        }
    }
}
