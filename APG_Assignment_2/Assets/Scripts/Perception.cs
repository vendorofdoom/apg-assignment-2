using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perception : MonoBehaviour
{
    public List<Collider> nearbyCuttles;
    public List<Collider> nearbyFood;
    public float visionDist;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cuttlefish") && other.isTrigger == false)
        {
            nearbyCuttles.Add(other);
        }
        else if (other.gameObject.CompareTag("Food"))
        {
            nearbyFood.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cuttlefish"))
        {
            nearbyCuttles.Remove(other);
        }
        else if (other.gameObject.CompareTag("Food"))
        {
            nearbyFood.Remove(other);
        }
    }

    public GameObject nearestCuttle(float distThreshold)
    {
        float minDist = Mathf.Infinity;
        GameObject nearestCuttle = null;

        foreach (Collider collider in nearbyCuttles)
        {
            float dist = Vector3.Distance(collider.transform.position, transform.position);
            if (dist < minDist && dist <= distThreshold)
            {
                minDist = dist;
                nearestCuttle = collider.gameObject;
            }
        }

        return nearestCuttle;
    }

    public Food nearestFood(float distThreshold)
    {
        // TODO: check if food is "owned" by another cuttle
        // TODO: check food "age", i.e. can we reach it before it disappears

        float minDist = Mathf.Infinity;
        Food nearestFood = null;

        foreach (Collider collider in nearbyFood)
        {
            if (collider != null)
            {
                float dist = Vector3.Distance(collider.transform.position, transform.position);
                if (dist < minDist && dist <= distThreshold)
                {
                    minDist = dist;
                    nearestFood = collider.gameObject.GetComponent<Food>();
                }
            }

        }

        return nearestFood;
    }

}
