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

    [SerializeField]
    private float currSpeed;

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

        currSpeed = rb.velocity.magnitude;
    }

    private void Update()
    {
        switch (movementState)
        {
            case MovementState.FollowPath:
                RotateTowards();
                RotateUpright();
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
        rb.AddForce(steer, ForceMode.Acceleration);
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

    private void Stop()
    {
        rb.velocity = Vector3.zero;
    }

    private void FollowPath()
    {
        if (path == null)
        {
            Debug.Log("no path to follow");
            return;
        }

        Vector3 predictedPos = transform.position + (rb.velocity.normalized * lookAhead);
        Vector3 normalPoint = ClosestNormalPoint(out int pathIdx, predictedPos); 

        // Attempt to fix issue where cuttlefish gets stuck between points, i.e. keep it moving down the path
        if (pathIdx < path.Count - 2 && normalPoint == path[pathIdx + 1])
        {
            pathIdx++;
        }

        Vector3 pathSegStart = path[pathIdx];
        Vector3 pathSegEnd = path[pathIdx + 1];

        // If we're at the end of the path make target the segment end point rather than looking ahead
        if (pathIdx == path.Count - 2)
        {
            target.position = pathSegEnd;
        }
        else
        {
            target.position = normalPoint + ((pathSegEnd - pathSegStart).normalized * lookAhead);
        }

        // debug   
        //Debug.DrawLine(pathSegStart, pathSegEnd);
        pathPredicted.position = predictedPos;
        pathNormal.position = normalPoint;

        if (pathIdx == path.Count - 2)
        {
            Arrive();
            CheckAtDestination();

        }
        else
        {
            if (Vector3.Distance(normalPoint, transform.position) > pathRadius || rb.velocity.magnitude < 0.1f)
            {
                Seek();
            }
        }
    }

    private void CheckAtDestination()
    {

        if ((Vector3.Distance(transform.position, target.position) < 0.1f) && (rb.velocity.magnitude < 0.1f))
        {
            Stop();
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