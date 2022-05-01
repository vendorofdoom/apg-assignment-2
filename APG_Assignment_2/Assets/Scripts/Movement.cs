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
    public Transform target;
    public Transform pathTarget;

    // path following
    public List<Vector3> path;

    public float lookAhead;

    // debug
    public Transform pathPredicted;
    public Transform pathNormal;
    public float pathRadius;

    public MovementState movementState;

    [SerializeField]
    private float currSpeed;

    public enum MovementState
    {
        FollowPath,
        Hover,
        Avoid,
        QuickEscape,
        Follow
    }

    private void FixedUpdate()
    {
        switch (movementState)
        {
            case MovementState.FollowPath:
                FollowPath();
                break;
            //case MovementState.Avoid:
            //    Avoid(target);
            //    break;
            case MovementState.QuickEscape:
                QuickEscape(target);
                break;
            case MovementState.Hover:
                Hover();
                break;
            case MovementState.Follow:
                Follow(target);
                break;
        }

        currSpeed = rb.velocity.magnitude;
    }

    private void Update()
    {
        switch (movementState)
        {
            case MovementState.FollowPath:
                RotateTowards(1f);
                CheckAtDestination(0.5f);
                break;
            case MovementState.Avoid:
                //RotateTowards(0f);
                break;
            case MovementState.QuickEscape:
                RotateTowards(0f);
                break;
            case MovementState.Follow:
                RotateTowards(0f);
                break;
        }

        RotateUpright();
    }

    private void Follow(Transform followTarget)
    {
        Vector3 targetPos = new Vector3(followTarget.position.x + Random.Range(-0.1f, 0.1f),
                                        followTarget.position.y + Random.Range(-0.1f, 0.1f),
                                        followTarget.position.z - 5f);
        


        Vector3 desiredVelocity = (targetPos - transform.position).normalized * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.Acceleration);
    }

    private void Seek(Transform seekTarget)
    {
        Vector3 desiredVelocity = (seekTarget.position - transform.position).normalized * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.Acceleration);
    }

    //private void Avoid(Transform avoidTarget)
    //{
    //    Debug.Log("Avoiding!");
    //    Vector3 desiredVelocity = (transform.position - avoidTarget.position).normalized * maxSpeed;
    //    Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
    //    rb.AddForce(steer, ForceMode.Acceleration);
    //}
    
    public void AvoidCollision(Vector3 collisionPoint)
    {
        Debug.Log("Avoiding! " + collisionPoint);
        Vector3 desiredVelocity = (transform.position - collisionPoint).normalized * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce * 2);
        rb.AddForce(steer, ForceMode.Acceleration);
    }


    private void QuickEscape(Transform escapeTarget)
    {
        Vector3 steer = (transform.position - escapeTarget.position).normalized * quickEscapeSpeed;
        rb.AddForce(steer, ForceMode.Impulse);
        movementState = MovementState.Hover;
    }

    private void Arrive(Transform arriveTarget)
    {
        Vector3 desiredVelocity = (arriveTarget.position - transform.position);
        float distanceToTarget = desiredVelocity.magnitude;
        desiredVelocity = desiredVelocity.normalized;

        if (distanceToTarget < stoppingDist)
        {
            float speed = Mathf.Lerp(0, maxSpeed, Mathf.InverseLerp(0, stoppingDist, distanceToTarget));
            
            desiredVelocity *= speed;
        }
        else
        {
            desiredVelocity *= maxSpeed;
        }

        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.Acceleration);
   
    }

    private void FollowPath()
    {
        if (path.Count == 0)
        {
            Debug.Log("no path to follow");
            movementState = MovementState.Hover;
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
            Arrive(pathTarget);
        }
        else
        { 
            if (Vector3.Distance(normalPoint, transform.position) > pathRadius || rb.velocity.magnitude < 0.1f)
            {
                Seek(pathTarget);

            }
        }

    }

    private void Hover()
    {
        if (rb.velocity.magnitude > 0.01f)
        {
            Vector3 steer = Vector3.ClampMagnitude(Vector3.zero - rb.velocity, maxForce);
            rb.AddForce(steer, ForceMode.Acceleration);
        }
        
    }

    private void CheckAtDestination(float distThreshold)
    {
        if (path.Count == 0)
            return;

        if ((Vector3.Distance(transform.position, path[path.Count-1]) < distThreshold))
        {
            Debug.Log("you have reached your destination");
            movementState = MovementState.Hover;
            path.Clear();
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


    private void RotateTowards(float distThreshold)
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