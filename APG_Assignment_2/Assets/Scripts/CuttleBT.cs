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

        Action rest = new Action(brain.Rest);
        Action wander = new Action(brain.Wander);
        Action goHome = new Action(brain.GoHome);
        Action ink = new Action(brain.Ink);
        Action goToNearestFood = new Action(brain.GoToNearestFood);
        Action eatFood = new Action(brain.EatFood);

        Sequence foodSequence = new Sequence(foodNearby, new Selector(new Sequence(foodCloseEnoughToEat, eatFood), goToNearestFood));
        Sequence inkSequence = new Sequence(beenPoked, ink);
        Sequence goHomeToRest = new Sequence(amTired, new Selector(new Sequence(atHome, rest), goHome));
        Sequence restAtHome = new Sequence(atHome, energyNotFull, rest);


        bt = new BehaviourTree(new Selector(inkSequence, foodSequence, goHomeToRest, restAtHome, rest));
    }

}
