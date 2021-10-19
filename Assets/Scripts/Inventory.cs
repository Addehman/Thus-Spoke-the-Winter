using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    public static Inventory Instance { get { return _instance; } }

    private PlayerController player;
    
    [SerializeField]
    public int currentInventory, inventoryMaxCapacity = 100;

    [SerializeField]
    private int wood, food, blueberry, lingonberry, apple, mushroom, venison;


    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;

        player = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        player.ResourceGathered += GatherResource;
        /*Subscribe to an event in classes that inherit from IInteractable.
        The event should send a string and an int to the inventory.
        The inventory then sees if it's something that should be added to the inventory, what that is, and how much that should be added.*/
    }

    void Start()
    {
        currentInventory = 0;
    }

    void GatherResource(GameObject obj)
    {
        print($"Food: {food}. Wood: {wood}");

        int gatheredWood = 0;
        int gatheredBlueberry = 0;
        int gatheredLingonberry = 0;
        int gatheredApple = 0;
        int gatheredMushroom = 0;
        int gatheredVenison = 0;


        if (currentInventory >= inventoryMaxCapacity)
        {
            print("Inventory is full");
            return;
        }

        var tree = obj.GetComponent<TreeBehaviour>();
        //Implement food script and remove comment below later.
        /*var food = obj.GetComponent<Food>();*/

        if (tree != null)
        {
            //These switch cases can be removed if we always att to "gatheredWood" as it is right now.
            //Keeping it for now if we want to add specific kinds of wood later on, for example; "gatheredPineWood" or "gatheredLeafTreeWood".
            switch (tree.type)
            {
                case ResourceType.fruitTree:
                    {
                        gatheredWood = tree.resourceAmount;
                        break;
                    }

                case ResourceType.leafTree:
                    {
                        gatheredWood = tree.resourceAmount;
                        break;
                    }

                case ResourceType.pineTree:
                    {
                        gatheredWood = tree.resourceAmount;
                        break;
                    }
                /*case ResourceType.newTree:
                    {
                        wood += newTreeAmount;
                        break;
                    }*/
            }
        }
        else if (food != null)
        {
            /*switch (food.type)
            {
                case ResourceType.blueberry:
                    {
                        gatheredBlueberry += food.resourceAmount;
                        break;
                    }

                case ResourceType.lingonberry:
                    {
                        gatheredLingonberry += food.resourceAmount;
                        break;
                    }

                case ResourceType.apple:
                    {
                        gatheredApple += food.resourceAmount;
                        break;
                    }
                case ResourceType.mushroom:
                    {
                        gatheredMushroom += food.resourceAmount;
                        break;
                    }
                case ResourceType.venison:
                    {
                        gatheredVenison += food.resourceAmount;
                        break;
                    }
            }*/
        }

        wood += gatheredWood;
        blueberry += gatheredBlueberry;
        lingonberry += gatheredLingonberry;
        apple += gatheredApple;
        mushroom += gatheredMushroom;
        venison += gatheredVenison;

        food = (blueberry + lingonberry + apple + mushroom + venison);


        InventoryCapacityFailsafe(gatheredWood, gatheredBlueberry, gatheredLingonberry, 
            gatheredApple, gatheredMushroom, gatheredVenison);


        currentInventory = (wood + food);

        print($"Food: {food}. Wood: {wood}");
    }

    /*void WhatResourceToAdd(GameObject obj)
    {
        //Try and implement and run this function in "GatherResource" to tidy up in "GatherResource".
    }*/


    void InventoryCapacityFailsafe(int gatheredWood, int gatheredBlueberry, int gatheredLingonberry, 
        int gatheredApple, int gatheredMushroom, int gatheredVenison)
    {
        if (inventoryMaxCapacity - (wood + food) < 0)
        {
            int correction = inventoryMaxCapacity - (wood + food);

            if (gatheredWood > 0)
            {
                //correction is a negative value, therefore we "add" the negative value to subtract.
                wood += correction;
            }
            else if (gatheredBlueberry > 0)
            {
                blueberry += correction;
            }
            else if (gatheredLingonberry > 0)
            {
                lingonberry += correction;
            }
            else if (gatheredApple > 0)
            {
                apple += correction;
            }
            else if (gatheredMushroom > 0)
            {
                mushroom += correction;
            }
            else if (gatheredVenison > 0)
            {
                venison += correction;
            }

            print("Inventory is full");
        }
    }
}
