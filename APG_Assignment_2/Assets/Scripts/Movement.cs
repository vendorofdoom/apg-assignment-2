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

    public Rigidbody rb;
    public Transform target;

    // path following
    public List<Vector3> path;

    public float lookAhead;

    // debug
    public Transform pathPredicted;
    public Transform pathNormal;
    public float pathRadius;

    public MovementState movementState;

    public enum MovementState
    {
        FollowPath,
        Escape,
        Hover
    }

    private void FixedUpdate()
    {
        switch (movementState)
        {
            case MovementState.FollowPath:
                FollowPath();
                break;
            case MovementState.Escape:
                break;
            case MovementState.Hover:
                break;
        }
    }

    private void Update()
    {
        switch (movementState)
        {
            case MovementState.FollowPath:
                RotateTowards();
                //RotateUpright();
                break;
            case MovementState.Hover:
                RotateUpright();
                break;
        }
    }

    private void Seek()
    {
        Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
        Vector3 steer = Vector3.ClampMagnitude(desiredVelocity - rb.velocity, maxForce);
        rb.AddForce(steer, ForceMode.VelocityChange);
    }

    private void Arrive()
    {
        Vector3 desiredVelocity = (target.position - transform.position);
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
        rb.AddForce(steer, ForceMode.VelocityChange);
   
    }

    private void FollowPath()
    {
        Vector3 predictedPos = transform.position + (rb.velocity.normalized * lookAhead);
        Vector3 normalPoint = ClosestNormalPoint(out int pathIdx, predictedPos); // TODO: fix issue where cuttlefish gets stuck between two equidistanct points :( 

        Vector3 pathSegStart = path[pathIdx];
        Vector3 pathSegEnd = path[pathIdx + 1];

        Vector3 targetPos = normalPoint + ((pathSegEnd - pathSegStart).normalized * lookAhead);
        target.position = targetPos;

        // debug   
        Debug.DrawLine(pathSegStart, pathSegEnd);
        pathPredicted.position = predictedPos;
        pathNormal.position = normalPoint;

        if (pathIdx == path.Count - 2)
        {
            Arrive();
            CheckAtDestination();

        }
        else
        {
            if (Vector3.Distance(normalPoint, transform.position) > pathRadius)
            {
                Seek();
            }
        }

        
    }

    private void CheckAtDestination()
    {

        if ((Vector3.Distance(transform.position, target.position) < 0.1f) && (rb.velocity.magnitude < 0.01f))
        {
            movementState = MovementState.Hover;
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
        return ((poi.x < Mathf.Max(start.x, end.x)) &&
                (poi.x > Mathf.Min(start.x, end.x)) &&
                (poi.y < Mathf.Max(start.y, end.y)) &&
                (poi.y > Mathf.Min(start.y, end.y)) &&
                (poi.z < Mathf.Max(start.z, end.z)) &&
                (poi.z > Mathf.Min(start.z, end.z)));
    }


    private void RotateTowards()
    {
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }


    private void RotateUpright()
    {
        Quaternion desiredRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}