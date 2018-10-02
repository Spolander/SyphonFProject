using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEngine : MonoBehaviour {

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

    public static SoundEngine instance;
    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlaySound(string name, Vector3 point, Transform parent)
    {
        GameObject sound = new GameObject();
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


        sound.transform.position = point;
        sound.transform.SetParent(parent);
        AS.dopplerLevel = 0;
        AS.spatialBlend = 1;
        AS.Play();
        Destroy(sound, AS.clip.length);
    }
}
