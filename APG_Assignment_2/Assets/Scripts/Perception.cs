using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perception : MonoBehaviour
{
    public List<Collider> nearbyCuttles;
    public List<Collider> nearbyFood;
    public float visionDist;

    [Header("Collision avoidance perception")]
    public float collisionCheckDist;
    public float collisionCheckRadius;
    public Transform potentialCollision;
    public Vector3 potentialCollisionPoint;
    public Rigidbody rb;
    public LayerMask layerMask;

    public bool AtHome;


    private void Update()
    {
        //LookWhereImGoing();
    }

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

    //public void LookWhereImGoing()
    //{
    //    //RaycastHit hitInfo;
    //    //Ray ray = new Ray(transform.position, rb.velocity);
    //    //if (Physics.Raycast(ray, out hitInfo, collisionCheckDist))

    //    RaycastHit hitInfo;
    //    if (Physics.SphereCast(transform.position, collisionCheckRadius, rb.velocity, out hitInfo, collisionCheckDist, layerMask))
    //    {
    //        potentialCollision = hitInfo.collider.transform;
    //        potentialCollisionPoint = hitInfo.point;
    //        //Debug.Log("Potential Collsion! " + potentialCollision.position + " " + hitInfo.collider.name);
    //    }
    //    else
    //    {
    //        potentialCollision = null;
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, collisionCheckRadius);
    //    Gizmos.DrawWireSphere(transform.position + (rb.velocity.normalized * collisionCheckDist), collisionCheckRadius);
    //}



    //public LayerMask layerMask;
    //public float fovAngle;

    //private bool InFOV(Transform other)
    //{

    //    if (Vector3.Angle(transform.forward, (other.position - transform.position)) * 2 <= fovAngle)
    //    {
    //        Debug.Log("Object in FOV: " + other.name);
    //        return true;
    //    }

    //    return false;

    //}

    //private bool CheckIfAnotherCuttleNearby()
    //{
    //    // Is there something nearby to avoid
    //    if (collidersNearby.Count > 0)
    //    {
    //        objToAvoid = nearestCuttle().transform;
    //        return true;
    //    }
    //    else
    //    {
    //        objToAvoid = null;
    //        return false;
    //    }
    //}



}
