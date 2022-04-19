using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleColour : MonoBehaviour
{

    public Color BaseColour1;
    public Color BaseColour2;
    public Color EyeColour;

    [Range(0f, 1f)]
    public float CamoLevel;
    [Range(0f, 1f)]
    public float PatternSlider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalColor("_CuttleBase1", BaseColour1);
        Shader.SetGlobalColor("_CuttleBase2", BaseColour2);
        Shader.SetGlobalColor("_CuttleEyeColour", EyeColour);
        Shader.SetGlobalFloat("_CuttleCamoLevel", CamoLevel);
        Shader.SetGlobalFloat("_CuttlePattern", PatternSlider);

    }
}
