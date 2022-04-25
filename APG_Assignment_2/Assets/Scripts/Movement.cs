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
    //public Transform target;
    public Tank tank;

    public MovementState movementState;
    public Vector3 steer;

    public List<Vector3> path; // TODO calc from graph later // path = tank.Graph.GetPath(transform.position, target.position);
    private Vector3 pathSegmentStart;
    private Vector3 pathSegmentEnd;
    public float pathRadius = 0.5f;
    public float lookAheadDist = 0.1f;

    public Transform target;
    public Transform debugPos;
    public float distToTarget;

    public int pathSegIdx = 0;

    public float steerMag;

    public enum MovementState
    {
        Hover,
        Wander,
        TowardsTarget

    }

    private void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPath();
        SteerTowardsTarget();
        UpdatePath();
    }

    private void UpdatePath()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < stoppingDist)
        {
            if (pathSegIdx < path.Count - 2)
            {
                pathSegIdx++;
            }
        }
    }

    private void FollowPath()
    {
        pathSegmentStart = path[pathSegIdx]; 
        pathSegmentEnd = path[pathSegIdx + 1]; 

        Debug.DrawLine(pathSegmentStart, pathSegmentEnd);

        Vector3 predictedPos = transform.position + (steer.normalized * lookAheadDist); 
        Vector3 pathSegStartToPredictedPos = (predictedPos - pathSegmentStart);
        Vector3 pathSegStartToSegEnd = (pathSegmentEnd - pathSegmentStart).normalized; 
        Vector3 pathSegStartToPoint = pathSegStartToSegEnd * (Vector3.Dot(pathSegStartToPredictedPos, pathSegStartToSegEnd)); 
        Vector3 pointOnPath = pathSegmentStart + pathSegStartToPoint;

        debugPos.transform.position = pointOnPath;

        if (Vector3.Distance(pointOnPath, predictedPos) >= pathRadius)
        {
            Vector3 targetPos = pointOnPath + (pathSegStartToSegEnd * lookAheadDist);
            if (!PointInsideLineSegment(pathSegmentStart, pathSegmentEnd, targetPos))
            {
                targetPos = pathSegmentEnd;
            }
            target.transform.position = targetPos;
        }
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

    private void SteerTowardsTarget()
    {
        Vector3 heading = (target.position - transform.position).normalized;

        distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget >= 0.01f)
        {
            
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
            steerMag = steer.magnitude;
            //rb.AddForce(steer * Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddForce(steer, ForceMode.Acceleration);

        }
    }

    
}