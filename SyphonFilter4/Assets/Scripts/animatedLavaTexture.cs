using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animatedLavaTexture : MonoBehaviour {

    public int width;
    public int height;

    Material m;
    public float cycleSpeed;
    WaitForSeconds delay;

    Vector2 size;
	// Use this for initialization
	void Start () {
        delay = new WaitForSeconds(cycleSpeed);
        m = GetComponent<Renderer>().material;

        size.x = 1.0f / width;
        size.y = 1.0f / height;
        StartCoroutine(cycle());
	}

    IEnumerator cycle()
    {
        float x = 1;
        float y = 1;
        while (1 == 1)
        {
            yield return delay;

            if (x < width)
                x++;
            else
            {
                if (y < height)
                    y++;
                else
                    y = 1;
                x = 1;
            }
            m.SetTextureOffset("_MainTex", new Vector2(x *size.x, y *size.y));
            m.SetTextureScale("_MainTex", size);
            
        }
    }
	
}
