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
        Null
    }


    // TODO: add action selection

    private void Start()
    {
        currAction = Action.Rest;
        prevAction = Action.Null;
    }

    private void Update()
    {
        if (currAction != prevAction)
        {
            prevAction = currAction;
            actionChanged = true;
        }

        if (actionChanged)
        {
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
            }

            actionChanged = false;
        }
            
        
    }

    private void GoHome()
    {
        cuttleColour.targetCamo = 0f;
        cuttleColour.targetPattern = 0.5f;
        movement.path = tank.Graph.GetPath(transform.position, homeLocation.position, true);
        movement.movementState = Movement.MovementState.FollowPath;
    }

    private void Rest()
    {
        cuttleColour.targetCamo = 0.5f;
        cuttleColour.targetPattern = 0.1f;
        movement.movementState = Movement.MovementState.Hover;
    }

    private void Ink()
    {
        cuttleColour.targetCamo = 0f;
        cuttleColour.targetPattern = 0f;
        movement.target.position = transform.position + transform.forward;
        movement.movementState = Movement.MovementState.QuickEscape;
        StartCoroutine("ReleaseInk");
    }

    private IEnumerator ReleaseInk()
    {
        ink.Play();
        yield return new WaitForSeconds(1f);
        ink.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        collidersNearby.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        collidersNearby.Remove(other);
    }


}
