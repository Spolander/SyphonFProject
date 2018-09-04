using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAnimation : MonoBehaviour {

    //planes that have the muzzleFlash texture
    [SerializeField]
    private Renderer[] muzzles;

    //muzzle parent objects for rotation
    [SerializeField]
    private Transform[] muzzleParents;

   //counter for interval
    float timer = 0;

    //how often to switch muzzle flash visibility
    float interval = 0.1f;

    //Are we currently animating the muzzle flashes
    private bool animating = false;
    public bool Animating { get { return animating; } }
    private void Start()
    {
        StopAnimating();
    }

   
    //called by the playerCombat when shooting
    public void Animate()
    {
        animating = true;
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0;

            //get the current texture offset
            Vector2 offset = muzzles[0].material.GetTextureOffset("_MainTex");

            //inveret the x offset
            offset.x = offset.x == 0 ? 1 : 0;

            //Change offset for all muzzle flash meshes
            for (int i = 0; i < muzzles.Length; i++)
            {

                muzzles[i].material.SetTextureOffset("_MainTex", offset);
            }

            //random rotation for muzzle parents
            for (int i = 0; i < muzzleParents.Length; i++)
            {
                muzzleParents[i].transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 359f));
            }

        }
    }

    //stops the animation and hide the texture with the offset
    public void StopAnimating()
    {
        animating = false;
        for (int i = 0; i < muzzles.Length; i++)
        {

            muzzles[i].material.SetTextureOffset("_MainTex", new Vector2(1,0));
        }

    }


    // Use this for initialization

}
