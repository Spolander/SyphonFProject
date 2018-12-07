using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundEngine : MonoBehaviour {

    [SerializeField]
    private AudioMixer mixer;

    [Space]

    [SerializeField]
    AudioClip BarrelExplosion;

    [SerializeField]
    AudioClip Door;

    [SerializeField]
    AudioClip HealthPickup;

    [SerializeField]
    AudioClip MovingPlatformSound;

    [SerializeField]
    AudioClip splash;

    [SerializeField]
    AudioClip[] waterWalk;

    [SerializeField]
    AudioClip[] Footsteps;

    [SerializeField]
    AudioClip[] pistolShots;

    [SerializeField]
    AudioClip[] grenadeSounds;

    [SerializeField]
    AudioClip[] mechaWeapons;

    [SerializeField]
    AudioClip[] mechaSteps;

    [SerializeField]
    AudioClip[] koiraFootSteps;

    [SerializeField]
    AudioClip[] swordSounds;

    [SerializeField]
    AudioClip[] swordImpact;

    [SerializeField]
    private AudioClip[] koiraHurt;

    [SerializeField]
    private AudioClip[] koiraDeath;

    [SerializeField]
    AudioClip[] shurikenSounds;
    [SerializeField]
    private AudioClip fleshImpact;
    private float lastShurikenSoundTime;

    public static SoundEngine instance;

   
    private void Awake()
    {
        instance = this;
    }

  
    public void StopSound(string name , Transform parent)
    {
        if (name == "MovingPlatformSound")
        {
            Destroy(parent.Find(name).gameObject);
        }
    }

    public void PlaySound(string name, Vector3 point, Transform parent)
    {
        GameObject sound = new GameObject(name);
        AudioSource AS = sound.AddComponent<AudioSource>();

        if (name == "splash")
        {
            AS.clip = splash;
        }
        else if (name == "waterWalk")
        {
            AS.clip = waterWalk[Random.Range(0, waterWalk.Length)];
        }
        else if (name == "pistolShot")
        {
            AS.clip = pistolShots[Random.Range(0, pistolShots.Length)];
        }
        else if (name == "Footsteps")
        {
            AS.clip = Footsteps[Random.Range(0, Footsteps.Length)];
            AS.maxDistance = 15;
        }
        else if (name == "grenade")
        {
            AS.clip = grenadeSounds[Random.Range(0, grenadeSounds.Length)];
            AS.maxDistance = 10;
            AS.minDistance = 2;
        }
        else if (name == "mechaweapons")
        {
            AS.clip = mechaWeapons[Random.Range(0, mechaWeapons.Length)];
            AS.minDistance = 1;
        }
        else if (name == "mechasteps")
        {
            AS.clip = mechaSteps[Random.Range(0, mechaSteps.Length)];
            AS.maxDistance = 10;
        }
        else if (name == "BarrelExplosion")
        {
            AS.clip = BarrelExplosion;
        }
        else if (name == "MovingPlatformSound")
        {
            AS.clip = MovingPlatformSound;
            AS.loop = true;
            AS.maxDistance = 15;
            AS.rolloffMode = AudioRolloffMode.Linear;
        }
        else if (name == "Door")
        {
            AS.clip = Door;
        }
        else if (name == "HealthPickup")
        {
            AS.clip = HealthPickup;
        }
        else if (name == "KoiraFootStep")
        {
            AS.clip = koiraFootSteps[Random.Range(0, koiraFootSteps.Length)];
        }
        else if (name == "koiraHurt")
        {
            AS.clip = koiraHurt[Random.Range(0, koiraHurt.Length)];
        }
        else if (name == "koiraDeath")
        {
            AS.clip = koiraDeath[Random.Range(0, koiraDeath.Length)];
        }
        else if (name == "sword")
        {
            AS.clip = swordSounds[Random.Range(0, swordSounds.Length)];
            AS.pitch += Random.Range(-0.1f, 0.1f);
        }
        else if (name == "shurikenThrow")
        {
            if (Time.time < lastShurikenSoundTime + 0.2f)
            {
                Destroy(AS.gameObject);
                return;
            }

            lastShurikenSoundTime = Time.time;
            AS.clip = shurikenSounds[0];
        }
        else if (name == "shurikenHit")
        {

            AS.clip = shurikenSounds[1];
        }
        else if (name == "swordImpact")
        {
            AS.clip = swordImpact[Random.Range(0, swordImpact.Length)];
        }
        else if (name == "fleshImpact")
        {
            AS.clip = fleshImpact;
        }

        AS.priority = 180;
        AS.outputAudioMixerGroup = mixer.FindMatchingGroups("FX")[0];
        sound.transform.position = point;
        sound.transform.SetParent(parent);
        AS.dopplerLevel = 0;
        AS.spatialBlend = 1;
        AS.Play();
        if(!AS.loop)
        Destroy(sound, AS.clip.length);
    }
}
