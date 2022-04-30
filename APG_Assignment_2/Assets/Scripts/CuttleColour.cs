using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleColour : MonoBehaviour
{

    public Color baseColour1;
    public Color baseColour2;

    [Range(0f, 1f)]
    public float camoLevel;
    [Range(0f, 1f)]
    public float patternSlider;

    [Range(0f, 1f)]
    public float targetCamo;
    [Range(0f, 1f)]
    public float targetPattern;

    public float colourChangeSpeed;


    // Update is called once per frame
    void Update()
    {

        camoLevel = Mathf.Lerp(camoLevel, targetCamo, Time.deltaTime * colourChangeSpeed);
        patternSlider = Mathf.Lerp(patternSlider, targetPattern, Time.deltaTime * colourChangeSpeed);

        Shader.SetGlobalColor("_CuttleBase1", baseColour1);
        Shader.SetGlobalColor("_CuttleBase2", baseColour2);
        Shader.SetGlobalColor("_CuttleEyeColour", baseColour1);
        Shader.SetGlobalFloat("_CuttleCamoLevel", camoLevel);
        Shader.SetGlobalFloat("_CuttlePattern", patternSlider);

    }
}
