using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleBrain : MonoBehaviour
{
    public Tank tank;
    public Movement movement;
    public CuttleColour cuttleColour;

    public Transform homeLocation;

    // Collision avoidance
    public Collider areaToAvoidCollisions;
    public List<Collider> collidersNearby;

    // World observations
    public Collider areaOfVision;


    [SerializeField]
    private Action currAction;
    private Action prevAction;
    private bool actionChanged;

    public ParticleSystem ink;

    private float energyChange;
    private float hungerChange;
    
    [SerializeField]
    private float energy = 1f;
    [SerializeField]
    private float hunger = 0f;

    private Transform objToAvoid;

    public enum Action
    {
        GoHome,
        Rest,
        Wander,
        Hide,
        Forage,
        Eat,
        Ink,
        InspectObject,
        FollowCursor,
        AvoidNearbyObj,
        Null
    }

    private void Start()
    {
        currAction = Action.Rest;
        prevAction = Action.Null;
    }

    private void Update()
    {
        SelectAction();
        PerformAction();
        UpdateStats();
    }

    private void SelectAction()
    {
        // TODO: Create behaviour tree for action selection
        if (collidersNearby.Count > 0)
        {
            objToAvoid = nearestObstacle().transform;
            currAction = Action.AvoidNearbyObj;
        }
        else if (energy < 0.5f)
        {
            if (Vector3.Distance(transform.position, homeLocation.position) > 1f)
            {
                currAction = Action.GoHome;
            }
            else
            {
                currAction = Action.Rest;
            }
        }
        else if (hunger > 0.5f)
        {
            currAction = Action.Eat;
        }
    }

    private void PerformAction()
    {
        if (currAction != prevAction)
        {
            prevAction = currAction;
            actionChanged = true;
        }


        switch (currAction)
        {
            case (Action.GoHome):
                GoHome();
                break;
            case (Action.Rest):
                Rest();
                break;
            case (Action.Ink):
                Ink();
                break;
            case (Action.Eat):
                Eat();
                break;
            case (Action.AvoidNearbyObj):
                AvoidObj();
                break;
        }
        actionChanged = false;
    }

    private void UpdateStats()
    {
        hunger = Mathf.Clamp01(hunger + hungerChange * Time.deltaTime);
        energy = Mathf.Clamp01(energy + energyChange * Time.deltaTime);
    }

    private void GoHome()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0.5f;
            hungerChange = 0.01f;
            movement.path = tank.Graph.GetPath(transform.position, homeLocation.position, true);
            movement.movementState = Movement.MovementState.FollowPath;
        }

    }

    private void Rest()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0.5f;
            cuttleColour.targetPattern = 0.1f;
            hungerChange = 0.005f;
            energyChange = 0.1f;
            movement.movementState = Movement.MovementState.Hover;
        }
    }

    private void Ink()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0f;
            hungerChange = 1f;
            energyChange = -1f;
            movement.target.position = transform.position + transform.forward;
            movement.movementState = Movement.MovementState.QuickEscape;
            StartCoroutine("ReleaseInk");
        }
    }

    private void Eat()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0.5f;
            cuttleColour.targetPattern = 0.1f;
            hungerChange = -1f;
            energyChange = 0.1f;
            movement.movementState = Movement.MovementState.Hover;

        }
    }

    private void AvoidObj()
    {
        movement.target.position = objToAvoid.position;
        movement.movementState = Movement.MovementState.Avoid;
    }


    private IEnumerator ReleaseInk()
    {
        ink.Play();
        yield return new WaitForSeconds(1f);
        ink.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collidersNearby.Add(collision.collider);
    }

    private void OnCollisionExit(Collision collision)
    {
        collidersNearby.Remove(collision.collider);
    }

    private Collider nearestObstacle()
    {
        float minDist = Mathf.Infinity;
        Collider nearestCollider = null;

        foreach (Collider collider in collidersNearby)
        {
            float dist = Vector3.Distance(collider.transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestCollider = collider;
            }
        }

        return nearestCollider;
    }
}
