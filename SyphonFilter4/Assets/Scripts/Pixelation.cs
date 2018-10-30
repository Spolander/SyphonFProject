using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Pixelation : MonoBehaviour
{

    public RenderTexture renderTexture;
    RenderTexture tex;
    Camera cam;
    bool pixel = false;



    public float heightMultiplier;

    int realRatio;
    void Start()
    {

        cam = GetComponent<Camera>();

        tex = new RenderTexture(renderTexture);
        
        realRatio = Mathf.RoundToInt(Screen.width / Screen.height);
        tex.width = NearestSuperiorPowerOf2(Mathf.RoundToInt(tex.width * realRatio));
        
    }
    private void Update()
    {
       // if(pixel)
      //  cam.targetTexture = tex;

        if (Input.GetKeyDown(KeyCode.P))
            changePixelation();

      
    }

    void changePixelation()
    {
        if (pixel)
        {
            cam.targetTexture = null;


        }


        else
        {
            cam.targetTexture = tex; 
        }
        pixel = !pixel;

    }
    void OnGUI()
    {
        if (pixel)
        {
            
           GUI.depth = 20;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);
            
        }
    }

    int NearestSuperiorPowerOf2(int n)
    {
        return (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(n) / Mathf.Log(2)));
    }
}