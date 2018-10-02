using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {

    

    public void Footstep()
    {
        if (GetComponent<PlayerCharacterController>().IsInWater == false)
        {
            SoundEngine.instance.PlaySound("Footsteps", transform.position, null);
        }
        else if (GetComponent<PlayerCharacterController>().IsInWater == true)
        {
        
            SoundEngine.instance.PlaySound("waterWalk", transform.position, null);
        }
    }
}
