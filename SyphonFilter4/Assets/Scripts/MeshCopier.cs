using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCopier : MonoBehaviour {

    SkinnedMeshRenderer smr;

    Shader transparent;

    [Header("Optional and looks terrible")]
    [SerializeField]
    private GameObject sword;
	// Use this for initialization
	void Start () {
        smr = GetComponent<SkinnedMeshRenderer>();
        transparent = Shader.Find("Legacy Shaders/Transparent/Bumped Diffuse");
	}
	
	// Update is called once per frame


    public void SpawnMeshCopy(float destroyDelay, float startAlpha)
    {
        StartCoroutine(meshCopyRoutine(destroyDelay, startAlpha));
    }

    IEnumerator meshCopyRoutine(float destroyDelay, float startAlpha)
    {

        //new copy of the player character mesh
        GameObject c = new GameObject("copied mesh");

        SkinnedMeshRenderer m = c.AddComponent<SkinnedMeshRenderer>();

        GameObject tempSword = null;
        Material tempSwordMaterial = null;
        //copy of the sword
        if (sword != null)
        {
            tempSword = Instantiate(sword);
            tempSword.transform.localScale = Vector3.one;
            tempSwordMaterial = tempSword.GetComponent<Renderer>().material;
        }


        Mesh mesh = new Mesh();
        smr.BakeMesh(mesh);

        m.sharedMesh = mesh;
        m.materials = smr.materials;

        m.materials[0].shader = transparent;
        m.materials[1].shader = transparent;

        c.transform.position = transform.position;
        c.transform.rotation = transform.rotation;


        if (tempSword != null)
        {
            tempSwordMaterial.shader = transparent;
            tempSword.transform.rotation = sword.transform.rotation;
            tempSword.transform.position = sword.transform.position;
        }

        float lerp = 0;

        while (lerp < 1)
        {
            lerp += Time.deltaTime / destroyDelay;
            Color color = m.materials[0].color;
            color.a = Mathf.Lerp(startAlpha, 0, lerp);
            m.materials[0].color = color;

            color = m.materials[1].color;
            color.a = Mathf.Lerp(startAlpha, 0, lerp);
            m.materials[1].color = color;

            if (tempSword != null)
            {
                color = tempSwordMaterial.color;
                color.a = Mathf.Lerp(startAlpha, 0, lerp);
                tempSwordMaterial.color = color;
            }

            yield return null;
        }

        Destroy(c);

        if (tempSword)
            Destroy(tempSword);
    }

}
