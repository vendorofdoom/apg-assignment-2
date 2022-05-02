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
        SelectAction();
    }

    private void FixedUpdate()
    {
        PerformAction(); // TODO: think where to put this, applying forces so maybe fixed update?
    }

    private void SelectAction()
    {
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
        }

        if (movement.AtPathDestination(1f))
        {
            movement.Hover();
            currAction = Action.Rest;
        }
        else
        {
            movement.FollowPath();
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
        }

        movement.Hover();
    }

    private void Ink()
    {
        if (actionChanged)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0f;
            //hunger = Mathf.Clamp01(hunger + 0.005f * Time.deltaTime);
            //energy = Mathf.Clamp01(energy - 0.1f * Time.deltaTime);
            movement.QuickEscape();
            StartCoroutine(ink.ReleaseInk());
        }
        else
        {
            // TODO: figure out how to handle inking so don't keep inking
            currAction = Action.Rest;
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
            movement.Hover();
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

        if (movement.AtPathDestination(1f) || actionChanged)
        {
            Vector3 randLoc = tank.Graph.Nodes.ToList()[Random.Range(0, tank.Graph.Nodes.Count)];
            movement.path = tank.Graph.GetPath(transform.position, randLoc, true);
        }

        movement.FollowPath();

    }

    private void FollowNearestCuttle()
    {

        if (actionChanged)
        {
                cuttleColour.targetCamo = 0f;
                cuttleColour.targetPattern = 1f;

        }

        GameObject nearestCuttle = perception.nearestCuttle(10f);

        if (nearestCuttle != null)
            movement.Follow(nearestCuttle.transform, -nearestCuttle.transform.forward * 5f);
        else
        {
            currAction = Action.Rest;
        }
    }


}
