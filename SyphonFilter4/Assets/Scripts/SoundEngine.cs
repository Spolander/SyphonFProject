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
        }



        sound.transform.position = point;
        sound.transform.SetParent(parent);
        AS.dopplerLevel = 0;
        AS.spatialBlend = 1;
        AS.Play();
        Destroy(sound, AS.clip.length);
    }
}
