using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlanZucconi.AI.BT;

public class CuttleBT : MonoBehaviour
{
    public CuttleBrain brain;
    private BehaviourTree bt;

    private void Start()
    {
        ConstructBT();
    }

    private void Update()
    {
        bt.Update();
    }



    private void ConstructBT()
    {

        Condition amTired = new Condition(brain.AmTired);
        Condition beenPoked = new Condition(brain.BeenPoked);
        Condition foodNearby = new Condition(brain.FoodNearby);
        Condition foodCloseEnoughToEat = new Condition(brain.FoodCloseEnoughToEat);
        Condition atHome = new Condition(brain.AtHome);
        Condition energyNotFull = new Condition(brain.EnergyNotFull);
        //Condition mouseOver = new Condition(brain.MouseOver);
        Condition cuttleNearby = new Condition(brain.AnotherCuttleNearby);
        Condition feelingPlayful = new Condition(brain.FeelingPlayful);

        Action rest = new Action(brain.Rest);
        Action wander = new Action(brain.Wander);
        Action goHome = new Action(brain.GoHome);
        Action ink = new Action(brain.Ink);
        Action goToNearestFood = new Action(brain.GoToNearestFood);
        Action eatFood = new Action(brain.EatFood);
        Action followNearbyCuttle = new Action(brain.FollowNearestCuttle);

        Sequence inkSequence = new Sequence(beenPoked, ink);
        Sequence foodSequence = new Sequence(foodNearby, new Selector(new Sequence(foodCloseEnoughToEat, eatFood), goToNearestFood));
        Sequence restAtHome = new Sequence(atHome, energyNotFull, rest);
        Sequence goHomeToRest = new Sequence(amTired, goHome);
        Sequence nearbyCuttleSequence = new Sequence(cuttleNearby, feelingPlayful, followNearbyCuttle);
        


        bt = new BehaviourTree(new Selector(inkSequence, foodSequence, restAtHome, goHomeToRest, nearbyCuttleSequence, wander));
    }

}
