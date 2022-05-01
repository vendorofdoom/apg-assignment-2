using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleFin : MonoBehaviour
{
    public Movement Movement;

    public float MinRippleFreq;
    public float MaxRippleFreq;

    [SerializeField]
    private float rippleFreq;

    public int cuttleID;
    
    // TODO: Decide if fin is flapping too much when changing direction

    void Update()
    {
        rippleFreq = Mathf.Lerp(MinRippleFreq, MaxRippleFreq, Mathf.InverseLerp(0, Movement.maxSpeed, Movement.rb.velocity.magnitude));

        if ( Movement.rb.velocity.z < 0)
        {
            rippleFreq *= -1;
        }
        Shader.SetGlobalFloat("_CuttleFinWaveFreq_" + cuttleID, rippleFreq);
    }
}
