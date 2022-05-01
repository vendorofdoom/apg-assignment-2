using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ink : MonoBehaviour
{
    public ParticleSystem ink;

    public IEnumerator ReleaseInk()
    {
        ink.Play();
        yield return new WaitForSeconds(1f);
        ink.Stop();
    }


}
