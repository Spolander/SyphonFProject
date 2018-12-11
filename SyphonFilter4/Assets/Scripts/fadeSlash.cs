using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeSlash : MonoBehaviour {

    // Use this for initialization

    public float fadeInDuration = 0.3f;

    public float fadeOutDuration = 0.3f;
	void Start () {
        StartCoroutine(Effect());
    }


    IEnumerator Effect()
    {

        float timer = 0f;

        Material m = GetComponent<Renderer>().material;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            Color c = m.GetColor("_TintColor");
            c.a = timer / fadeInDuration;
            m.SetColor("_TintColor", c);
            yield return null;
        }

        timer = 0;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            Color c = m.GetColor("_TintColor");
            c.a = 1-(timer / fadeOutDuration);
            m.SetColor("_TintColor", c);
            yield return null;
        }


        Destroy(gameObject);
    }
}
