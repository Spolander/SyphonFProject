using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashAnimation : MonoBehaviour {

    [SerializeField]
    private Renderer[] muzzles;

    [SerializeField]
    private Transform[] muzzleParents;

    float a = 0;
    float timer = 0;
    float interval = 0.1f;
    private void Start()
    {
        StopAnimating();
    }

   
    public void Animate()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            Vector2 offset = muzzles[0].material.GetTextureOffset("_MainTex");
            offset.x = offset.x == 0 ? 1 : 0;
            for (int i = 0; i < muzzles.Length; i++)
            {

                muzzles[i].material.SetTextureOffset("_MainTex", offset);
            }

            for (int i = 0; i < muzzleParents.Length; i++)
            {
                muzzleParents[i].transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 359f));
            }

        }
    }

    public void StopAnimating()
    {
        for (int i = 0; i < muzzles.Length; i++)
        {

            muzzles[i].material.SetTextureOffset("_MainTex", new Vector2(1,0));
        }

    }


    // Use this for initialization

}
