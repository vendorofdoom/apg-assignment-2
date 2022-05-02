using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tutorials:
// https://natureofcode.com/book/chapter-6-autonomous-agents/

public class Movement : MonoBehaviour
{
    public float maxSpeed;
    public float maxForce;
    public float rotationSpeed;
    public float stoppingDist;
    public float quickEscapeSpeed;

    public Rigidbody rb;

    // path following
    public List<Vector3> path;
    public float pathRadius;
    public float lookAhead;

    // debug
    public Transform pathTarget;
    public Transform pathPredicted;
    public Transform pathNormal;

    public float currSpeed;

    public LayerMask collisionAvoidanceLayerMask;
    public Vector3 collisionAvoidanceBBox;

    private void Update()
    {
        RotateUpright();
    }

    private void FixedUpdate()
    {
        currSpeed = rb.velocity.magnitude; 
    }

    public void Hover()
    {
        if (rb.velocity.magnitude > 0.01f)
        {
            Vector3 steer = Vector3.ClampMagnitude(Vector3.zero - rb.velocity, maxForce);
            rb.AddForce(steer, ForceMode.Acceleration);
        }
    }

    public void GoDown()
    {
        Vector3 desiredVelocity = Vector3.down * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.Acceleration);
    }

    public void QuickEscape()
    {
        Vector3 steer = -transform.forward * quickEscapeSpeed;
        rb.AddForce(steer, ForceMode.Impulse);
    }

    public void Follow(Transform target, Vector3 offset)
    {
        Vector3 targetPos = target.position + offset;
        Vector3 desiredVelocity = (targetPos - transform.position).normalized * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.Acceleration);
        RotateTowards(target, 0f);
    }

    public void FollowPath()
    {
        if (path.Count == 0)
        {
            Debug.Log("no path to follow");
            return;
        }

        Vector3 predictedPos = transform.position + (rb.velocity.normalized * lookAhead);
        Vector3 normalPoint = ClosestNormalPoint(out int pathIdx, predictedPos);

        // useful for debugging   
        pathPredicted.position = predictedPos;
        pathNormal.position = normalPoint;

        Vector3 pathSegStart = path[pathIdx];
        Vector3 pathSegEnd = path[pathIdx + 1];

        pathTarget.position = normalPoint + ((pathSegEnd - pathSegStart).normalized * lookAhead);


        // If we're at the end of the path make target the segment end point rather than looking ahead
        if (pathIdx == path.Count - 2)
        {
            if (!PointInsideLineSegment(pathSegStart, pathSegEnd, pathTarget.position))
            {
                pathTarget.position = pathSegEnd;
            }

            RotateTowards(pathTarget, 0.5f);
            rb.AddForce(ArriveAndAvoidCollisions(pathTarget), ForceMode.Acceleration);

        }
        else
        {
            if (Vector3.Distance(normalPoint, transform.position) > pathRadius || rb.velocity.magnitude < 0.1f)
            {
                RotateTowards(pathTarget, 0f);
                rb.AddForce(SeekAndAvoidCollisions(pathTarget), ForceMode.Acceleration);

            }
        }
    }

    private Vector3 AverageSteer(List<Vector3> steersToCombine)
    {
        Vector3 average = Vector3.zero;

        if (steersToCombine.Count > 0)
        {
            foreach (Vector3 v in steersToCombine)
            {
                average += v;
            }
            average /= steersToCombine.Count;
        }

        return average;

    }

    private Vector3 SeekAndAvoidCollisions(Transform target)
    {
        List<Vector3> desiredVelocities = CollisionAvoidanceSteers(10);
        desiredVelocities.Add(target.position - transform.position);

        Vector3 steer = AverageSteer(desiredVelocities);
        steer = steer.normalized * maxSpeed;
        steer = Vector3.ClampMagnitude(steer - rb.velocity, maxForce);
        return steer;
        //rb.AddForce(steer, ForceMode.Acceleration);
    }


    private Vector3 ArriveAndAvoidCollisions(Transform target)
    {
        List<Vector3> desiredVelocities = CollisionAvoidanceSteers(10);
        Vector3 desiredVelocity = (target.position - transform.position);
        desiredVelocities.Add(desiredVelocity);
        float distanceToTarget = desiredVelocity.magnitude;
        Vector3 steer = AverageSteer(desiredVelocities);

        steer = steer.normalized;

        if (distanceToTarget < stoppingDist)
        {
            float speed = Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(0, stoppingDist, distanceToTarget));

            steer *= speed;
        }
        else
        {
            steer *= maxSpeed;
        }

        steer = Vector3.ClampMagnitude(steer - rb.velocity, maxForce);
        return steer;
        //rb.AddForce(steer, ForceMode.Acceleration);
    }



    //private Vector3 Seek(Transform target)
    //{
    //    Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
    //    Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
    //    return steer;
    //    //rb.AddForce(steer, ForceMode.Acceleration);
    //}

    //private Vector3 Arrive(Transform target)
    //{
    //    Vector3 desiredVelocity = (target.position - transform.position);
    //    float distanceToTarget = desiredVelocity.magnitude;
    //    desiredVelocity = desiredVelocity.normalized;

    //    if (distanceToTarget < stoppingDist)
    //    {
    //        float speed = Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(0, stoppingDist, distanceToTarget));

    //        desiredVelocity *= speed;
    //    }
    //    else
    //    {
    //        desiredVelocity *= maxSpeed;
    //    }

    //    Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
    //    return steer;
    //    //rb.AddForce(steer, ForceMode.Acceleration);
    //}

    private List<Vector3> CollisionAvoidanceSteers(int maxColliders)
    {
        List<Vector3> steers = new List<Vector3>();

        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapBoxNonAlloc(transform.position, collisionAvoidanceBBox, 
                                                      hitColliders, transform.rotation, 
                                                      collisionAvoidanceLayerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].gameObject != this.gameObject)
            {
                //Debug.Log("Avoiding! " + hitColliders[i].name);
                Vector3 desiredVelocity = (transform.position - hitColliders[i].ClosestPoint(transform.position)).normalized * maxSpeed;
                Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
                steers.Add(steer);
            }

        }

        return steers;

        //rb.AddForce(steer, ForceMode.Acceleration);
    }

    public bool AtPathDestination(float distThreshold)
    {
        if (path.Count == 0)
        {
            //Debug.Log("path is empty");
            return true;
        }
        else if ((Vector3.Distance(transform.position, path[path.Count - 1]) < distThreshold))
        {
            //Debug.Log("you have reached your destination");
            path.Clear();
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetNormalPoint(Vector3 start, Vector3 end, Vector3 poi)
    {
        Vector3 a = poi - start;
        Vector3 b = (end - start).normalized;
        b *= Vector3.Dot(a, b);
        Vector3 normalPoint = start + b;

        return normalPoint;
    }

    private Vector3 ClosestNormalPoint(out int pathIdx, Vector3 poi)
    {
        Vector3 normalPoint;
        Vector3 closestNormalPoint = Vector3.zero;
        pathIdx = -1;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < path.Count - 1; i++)
        {
            normalPoint = GetNormalPoint(path[i], path[i + 1], poi);
            if (!PointInsideLineSegment(path[i], path[i + 1], normalPoint))
            {
                normalPoint = path[i + 1];
            }

            float dist = Vector3.Distance(normalPoint, poi);
            if (dist < minDist)
            {
                minDist = dist;
                pathIdx = i;
                closestNormalPoint = normalPoint;
            }

        }

        return closestNormalPoint;

    }

    private bool PointInsideLineSegment(Vector3 start, Vector3 end, Vector3 poi)
    {
        float sp = (start - poi).sqrMagnitude;
        float pe = (poi - end).sqrMagnitude;
        float se = (start - end).sqrMagnitude;

        return ((sp + pe) - se <= 0.001f);
    }

    private void RotateTowards(Transform target, float distThreshold)
    {
        Vector3 lookDir = (target.position - transform.position);
        if (lookDir.magnitude > distThreshold)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void RotateUpright()
    {
        Quaternion desiredRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }

}