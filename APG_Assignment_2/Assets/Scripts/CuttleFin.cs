using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleFin : MonoBehaviour
{
    public Movement movement;

    public float minRippleFreq;
    public float maxRippleFreq;

    public float changeSpeed;

    [SerializeField]
    private float rippleFreq;

    public int cuttleID;
    
    void Update()
    {
        float currRippleFreq = Shader.GetGlobalFloat("_CuttleFinWaveFreq_" + cuttleID);
        rippleFreq = Mathf.Lerp(minRippleFreq, maxRippleFreq, Mathf.InverseLerp(0, movement.maxSpeed, movement.currSpeed));


        rippleFreq = Mathf.Lerp(currRippleFreq, rippleFreq, Time.deltaTime * changeSpeed);

        Shader.SetGlobalFloat("_CuttleFinWaveFreq_" + cuttleID, rippleFreq);
    }
}
