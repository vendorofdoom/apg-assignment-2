using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleFin : MonoBehaviour
{
    public Movement Movement;

    public float MinRippleFreq;
    public float MaxRippleFreq;

    
    // TODO: Decide if fin is flapping too much when changing direction

    void Update()
    {
        float rippleFreq = Mathf.Lerp(MinRippleFreq, MaxRippleFreq, Mathf.InverseLerp(0, Movement.maxSpeed, Movement.rb.velocity.magnitude));

        if ( Movement.steer.z < 0)
        {
            rippleFreq *= -1;
        }
        Shader.SetGlobalFloat("_CuttleFinWaveFreq", rippleFreq);
    }
}
