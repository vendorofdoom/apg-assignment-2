using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tutorials:
// https://natureofcode.com/book/chapter-6-autonomous-agents/

public class Movement : MonoBehaviour
{

    public float maxSpeed;
    public float maxForce;
    public float stoppingDist;
    public float maxTurningDelta;

    public Rigidbody rb;
    public Transform target;

    public Vector3 steer;

    // Update is called once per frame
    void FixedUpdate()
    {
        SteerTowardsTarget();
    }

    private void SteerTowardsTarget()
    {
        Vector3 heading = (target.position - transform.position).normalized;

        float distToTarget = Vector3.Distance(transform.position, target.position);
        if (distToTarget <= stoppingDist)
        {
            heading *= Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(0, stoppingDist, distToTarget));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation((target.position - transform.position), Vector3.up), 
                Mathf.Lerp(0, maxTurningDelta, Mathf.InverseLerp(0, stoppingDist, distToTarget)));
        }
        else
        {
            heading *= maxSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((target.position - transform.position), Vector3.up), maxTurningDelta);
        }

        steer = Vector3.ClampMagnitude((heading - rb.velocity), maxForce);
        rb.AddForce(steer);
    }

}