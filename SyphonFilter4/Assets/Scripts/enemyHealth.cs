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

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public override void takeDamage(float amount, GameObject caller)
    {
        Health = Health - amount;
        Debug.Log(Health);

        anim.Play("Hurt",0,0.0f);
       
        Quaternion lookRotation =Quaternion.LookRotation(-Vector3.Scale(caller.transform.position - transform.position, new Vector3(1, 0, 1)));

        if (bloodParticle && Random.value < chanceToSpawnBlood)
        {
            GameObject g = (GameObject)Instantiate(bloodParticle, transform.position + Vector3.up,lookRotation);
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
            SoundEngine.instance.PlaySound("koiraHurt", transform.position, transform);
            SoundEngine.instance.PlaySound("fleshImpact", transform.position, transform);

            if (GetComponent<BaseAI>())
            GetComponent<BaseAI>().LastAggroTime = Time.time;
        }
    }

 

    public override void death(GameObject caller)
    {

        SoundEngine.instance.PlaySound("koiraDeath", transform.position, transform);
        if (anim)
        {
           // GetComponent<BaseAI>().enabled = false;
            GetComponent<Collider>().enabled = false;
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().isKinematic = true;
            anim.Play("Death");
            Destroy(gameObject);

            if (GetComponent<DogAI>())
                GetComponent<DogAI>().enabled = false;
        }
        else
            Destroy(gameObject);
    }
}
