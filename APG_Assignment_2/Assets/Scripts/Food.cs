using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float age;
    public float lifetime;

    private Tank tank;

    //private float nutritionalValue;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        age = 0f;
        tank = GameObject.FindGameObjectWithTag("Tank").GetComponent<Tank>();
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (age >= lifetime)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale / 4, Mathf.InverseLerp(0, lifetime, age));
        }
    }

    public void Consume()
    {
        Debug.Log("yum!");
        GameObject.Destroy(this.gameObject);
    }
}
