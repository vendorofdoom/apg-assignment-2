using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleColour : MonoBehaviour
{
    public int cuttleID = 0;

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

        Shader.SetGlobalColor("_CuttleBase1_" + cuttleID, baseColour1);
        Shader.SetGlobalColor("_CuttleBase2_" + cuttleID, baseColour2);
        Shader.SetGlobalColor("_CuttleEyeColour_" + cuttleID, baseColour2);
        Shader.SetGlobalFloat("_CuttleCamoLevel_" + cuttleID, camoLevel);
        Shader.SetGlobalFloat("_CuttlePattern_" + cuttleID, patternSlider);

    }
}
