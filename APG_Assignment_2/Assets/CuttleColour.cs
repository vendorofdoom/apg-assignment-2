using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleColour : MonoBehaviour
{

    public Color BaseColour1;
    public Color BaseColour2;
    public Color EyeColour;

    public RenderTexture CamoCamTexture;  // TODO: Should we try to do this in shader? Can we have finer control over the transition in the shader? 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        Shader.SetGlobalColor("_CuttleBase1", BaseColour1);
        Shader.SetGlobalColor("_CuttleBase2", BaseColour2);

    }
}
