using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleFin : MonoBehaviour
{
    public Movement Movement;

    public float MinRippleFreq;
    public float MaxRippleFreq;
    public float rippleChangeSpeed;

    [SerializeField]
    private float rippleFreq;

    public int cuttleID;
    
    void Update()
    {
        float targetRipple = Mathf.Lerp(MinRippleFreq, MaxRippleFreq, Mathf.InverseLerp(0, Movement.maxSpeed, Movement.rb.velocity.magnitude));
        float velo = 0;
        rippleFreq = Mathf.SmoothDamp(rippleFreq, targetRipple, ref velo, Time.deltaTime, rippleChangeSpeed);
        Shader.SetGlobalFloat("_CuttleFinWaveFreq_" + cuttleID, rippleFreq);
    }
}
