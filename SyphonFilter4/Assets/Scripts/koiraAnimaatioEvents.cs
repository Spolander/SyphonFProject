using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class koiraAnimaatioEvents : MonoBehaviour {

    public void FootStepSound()
    {
        SoundEngine.instance.PlaySound("KoiraFootStep", transform.position, null);
    }
}
