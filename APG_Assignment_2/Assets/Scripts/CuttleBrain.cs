using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CuttleBrain : MonoBehaviour
{
    public int cuttleID;

    public Tank tank;
    public Movement movement;
    public CuttleColour cuttleColour;
    public CuttleFin cuttleFin;
    public Ink ink;
    public Perception perception;

    public Transform homeLocation;


    [SerializeField]
    private Action currAction;
    private Action prevAction;
    private bool actionChanged;

    // status
    [SerializeField]
    private float energy = 1f;
    [SerializeField]
    private float hunger = 0f;

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
        AvoidCollison,
        FollowNearestCuttle,
        Null
    }

    private void Awake()
    {
        cuttleColour.cuttleID = cuttleID;
        cuttleFin.cuttleID = cuttleID;
    }

    private void Start()
    {
        currAction = Action.Rest;
        prevAction = Action.Null;
    }

    private void Update()
    {
        AvoidCollison();
        SelectAction();
        //perception.Perceive();
        PerformAction();
    }

    private void SelectAction()
    {
    }

    //private void SelectAction()
    //{
    //    // TODO: Create behaviour tree for action selection
    //    if (collidersNearby.Count > 0)
    //    {
    //        objToAvoid = nearestCuttle().transform;
    //        currAction = Action.AvoidNearbyObj;
    //    }

    //    else if (collidersNearby.Count == 0 && currAction == Action.AvoidNearbyObj)
    //    {
    //        currAction = Action.Rest;
    //    }

    //    else if (energy < 0.5f)
    //    {
    //        if (Vector3.Distance(transform.position, homeLocation.position) > 0.5f)
    //        {
    //            currAction = Action.GoHome;
    //        }
    //        else
    //        {
    //            currAction = Action.Rest;
    //        }
    //    }
    //    else if (hunger > 0.5f)
    //    {
    //        currAction = Action.Eat;
    //    }

    //}


    private void PerformAction()
    {
        if (currAction != prevAction)
        {
            prevAction = currAction;
            actionChanged = true;
        }


        switch (currAction)
        {
            case Action.GoHome:
                GoHome();
                break;
            case Action.Rest:
                Rest();
                break;
            case Action.Ink:
                Ink();
                break;
            case Action.Eat:
                Eat();
                break;
            case Action.FollowNearestCuttle:
                FollowNearestCuttle();
                break;
            case Action.Wander:
                Wander();
                break;
            case Action.AvoidCollison:
                AvoidCollison();
                break;
        }

        actionChanged = false;
    }


    private void GoHome()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0.5f;
            //hunger = Mathf.Clamp01(hunger + 0.01f * Time.deltaTime);
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
            //hunger = Mathf.Clamp01(hunger + 0.005f * Time.deltaTime);
            //energy = Mathf.Clamp01(energy + 0.1f * Time.deltaTime);
            movement.movementState = Movement.MovementState.Hover;
        }
    }

    private void Ink()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0f;
            //hunger = Mathf.Clamp01(hunger + 0.005f * Time.deltaTime);
            //energy = Mathf.Clamp01(energy - 0.1f * Time.deltaTime);
            movement.target = movement.pathTarget;
            movement.target.position = transform.position + transform.forward;
            movement.movementState = Movement.MovementState.QuickEscape;
            StartCoroutine(ink.ReleaseInk());
        }
    }

    private void Eat()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0.5f;
            cuttleColour.targetPattern = 0.1f;
            //hunger = Mathf.Clamp01(hunger + 0.005f * Time.deltaTime);
            //energy = Mathf.Clamp01(energy - 0.1f * Time.deltaTime);
            movement.movementState = Movement.MovementState.Hover;

        }
    }
    
    private void Wander()
    {
        if (actionChanged)
        { 
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0.5f;

            //hunger = Mathf.Clamp01(hunger + 0.005f * Time.deltaTime);
            //energy = Mathf.Clamp01(energy - 0.1f * Time.deltaTime);
        }

        if (movement.path.Count == 0 || actionChanged)
        {
            Vector3 randLoc = tank.Graph.Nodes.ToList()[Random.Range(0, tank.Graph.Nodes.Count)];
            movement.path = tank.Graph.GetPath(transform.position, randLoc, true);
            movement.movementState = Movement.MovementState.FollowPath;
        }
    }

    private void FollowNearestCuttle()
    {
        if (actionChanged)
        {
            GameObject nearestCuttle = perception.nearestCuttle(10f);
            if (nearestCuttle != null)
                movement.target = nearestCuttle.transform;
                movement.movementState = Movement.MovementState.Follow;
        }

    }

    private void AvoidCollison()
    {
        if (perception.potentialCollision != null)
        {
            movement.AvoidCollision(perception.potentialCollisionPoint);
        }
    }




}
