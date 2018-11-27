using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {

    [SerializeField]
    private MeshCopier playerMeshCopier;

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

    //calls the mesh copier to create a copy of the player mesh and fade it out
    public void CreateMeshCopy()
    {
        if (playerMeshCopier)
        {
            playerMeshCopier.SpawnMeshCopy(0.4f, 0.8f);
        }
    }
}
