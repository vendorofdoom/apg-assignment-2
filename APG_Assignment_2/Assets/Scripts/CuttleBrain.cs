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
    public Transform homeLocation;

    public LayerMask groundLayerMask;
    public float minGroundDistToRest;

    public float minCuttleDist;
    public float minSeeFoodDist;
    public float minEatFoodDist;

    // status
    [SerializeField]
    private float energy;
    [SerializeField]
    private float hunger;
    [SerializeField]
    private Mood mood;
    public Mood[] moods;

    private float timeToNextMoodChange;

    public Action action; // attempt to debug behaviour

    public enum Mood
    {
        Playful,
        Neutral
    }


    public enum Action
    {
        Ink,
        FollowCuttle,
        Wander,
        Rest,
        GoHome,
        GoToFood,
        Eat,
        MoveDown
    }

    private void Awake()
    {
        cuttleColour.cuttleID = cuttleID;
        cuttleFin.cuttleID = cuttleID;
        mood = moods[Random.Range(0, moods.Length)];
        energy = Random.Range(0f, 1f);
        hunger = Random.Range(0f, 1f);
    }

    private void Update()
    {
        if (timeToNextMoodChange <= 0f)
        {
            timeToNextMoodChange = Random.Range(5f, 10f);
            mood = moods[Random.Range(0, moods.Length)];
        }
        {
            timeToNextMoodChange -= Time.deltaTime;
        }
    }

    // Conditions
    public bool AmTired()
    {
        return energy <= 0.05f;
    }

    public bool EnergyNotFull()
    {
        return energy < 1f;
    }

    public bool AtHome()
    {
        return (Vector3.Distance(transform.position, homeLocation.position) <= 0.5f);
    }

    public bool AwayFromHome()
    {
        return (Vector3.Distance(transform.position, homeLocation.position) >= 10f);
    }

    public bool Hungry()
    {
        return hunger >= 0.5f;
    }

    public bool BeenPoked()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (gameObject.GetComponent<Collider>().Raycast(ray, out hitInfo, 100f))
            {
                //Debug.Log("Clicked!");
                return true;
            }
        }

        return false;
    }

    public bool FeelingPlayful()
    {
        return (mood == Mood.Playful);
    }

    public bool AnotherCuttleNearby()
    {
        GameObject nearestCuttle = NearestCuttle(minCuttleDist);
        if (nearestCuttle != null)
        {
            return true;
        }
        return false;
    }

    public bool MouseOver()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (gameObject.GetComponent<Collider>().Raycast(ray, out hitInfo, 100f))
        {
            Debug.Log("Mouse over!");
            return true;
        }

        return false;
    }

    public bool FoodNearby()
    {
        Food nearestFood = NearestFood(minSeeFoodDist);
        if (nearestFood != null)
        {
            return true;
        }
        return false;
    }

    public bool FoodCloseEnoughToEat()
    {
        Food nearestFood = NearestFood(minEatFoodDist);
        if (nearestFood != null)
        {
            return true;
        }
        return false;
    }

    public bool CloseEnoughToGroundToRest()
    {
        if (Physics.Raycast(transform.position, Vector3.down, minGroundDistToRest, groundLayerMask, QueryTriggerInteraction.Collide)) // check no obstacles "blocking view"
        {
            return true;
        } 
        return false;
    }

    // Actions
    public void Rest()
    {
        action = Action.Rest;

        energy = Mathf.Clamp01(energy + 0.1f * Time.deltaTime);
        hunger = Mathf.Clamp01(hunger + 0.05f * Time.deltaTime);

        cuttleColour.targetCamo = 0.8f;
        cuttleColour.targetPattern = 0.1f;
        movement.Hover();
    }

    public void MoveDown()
    {
        action = Action.MoveDown;

        cuttleColour.targetCamo = 0.5f;
        cuttleColour.targetPattern = 0.1f;

        movement.GoDown();
    }

    public void Wander()
    {
        bool prevWandering = (action == Action.Wander);

        action = Action.Wander; 

        energy = Mathf.Clamp01(energy - 0.01f * Time.deltaTime);
        hunger = Mathf.Clamp01(hunger + 0.01f * Time.deltaTime);

        if (movement.AtPathDestination(1f) || !prevWandering)
        {
            Vector3 randLoc = tank.Graph.Nodes.ToList()[Random.Range(0, tank.Graph.Nodes.Count)];
            movement.path = tank.Graph.GetPath(transform.position, randLoc, true);
        }

        cuttleColour.targetCamo = 0f;
        cuttleColour.targetPattern = 0.5f;
        movement.FollowPath();
    }
    
    public void GoHome()
    {
        action = Action.GoHome;

        if (movement.path.Count == 0 || (movement.path[movement.path.Count - 1] != homeLocation.position))
        {
            movement.path = tank.Graph.GetPath(transform.position, homeLocation.position, true);
        }

        if (!movement.AtPathDestination(1f))
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 0.5f;
            movement.FollowPath();
        }
    }

    public void Ink()
    {
        action = Action.Ink;

        energy = Mathf.Clamp01(energy - 0.5f); // don't use time.deltatime as it's a one off action
        hunger = Mathf.Clamp01(hunger + 0.1f);

        cuttleColour.targetCamo = 0f;
        cuttleColour.targetPattern = 1f;
        movement.QuickEscape();
        StartCoroutine(ink.ReleaseInk());
    }


    public void GoToNearestFood()
    {
        action = Action.GoToFood;

        Food nearestFood = NearestFood(minSeeFoodDist);
        if (nearestFood != null)
        {
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 1f;
            movement.Follow(nearestFood.transform, Vector3.zero);
        }
    }

    public void EatFood()
    {
        action = Action.Eat;

        Food nearestFood = NearestFood(minEatFoodDist);
        if (nearestFood != null)
        {
            movement.Hover();
            tank.availableFood.Remove(nearestFood);
            nearestFood.Consume();
            hunger = Mathf.Clamp01(hunger - 0.5f * Time.deltaTime);
        }
    }

    public void FollowNearestCuttle()
    {
        action = Action.FollowCuttle;

        GameObject nearestCuttle = NearestCuttle(minCuttleDist);
        if (nearestCuttle != null)
        {
            Debug.Log("yo");
            cuttleColour.targetCamo = 0f;
            cuttleColour.targetPattern = 1f;
            movement.Follow(nearestCuttle.transform, -nearestCuttle.transform.forward * 5f);
        }
    }


    // Utility
    public GameObject NearestCuttle(float distThreshold)
    {
        float minDist = Mathf.Infinity;
        GameObject nearestCuttle = null;

        foreach (CuttleBrain cuttle in tank.cuttlesInTank)
        {
            if (cuttle != this)
            {
                float dist = Vector3.Distance(cuttle.transform.position, transform.position);
                if (dist < minDist && dist <= distThreshold)
                {
                    minDist = dist;
                    nearestCuttle = cuttle.gameObject;
                }
            }
        }

        return nearestCuttle;
    }

    private Food NearestFood(float distThreshold)
    {

        float minDist = Mathf.Infinity;
        Food nearestFood = null;

        foreach (Food food in tank.availableFood)
        {
            if (food != null)
            {                
                float dist = Vector3.Distance(food.transform.position, transform.position);

                if (!Physics.Raycast(transform.position, (food.transform.position - transform.position), distThreshold, movement.collisionAvoidanceLayerMask)) // check no obstacles "blocking view"
                {
                    if (dist < minDist && dist <= distThreshold)
                    {
                        minDist = dist;
                        nearestFood = food;
                    }
                }
            }
        }

        return nearestFood;
    }

}
