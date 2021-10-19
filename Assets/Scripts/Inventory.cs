using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    public static Inventory Instance { get { return _instance; } }

    private PlayerController player;
    
    [SerializeField]
    public int currentInventory;
    [SerializeField]
    public int inventoryMaxCapacity = 100;


    [SerializeField]
    private int wood;
    [SerializeField]
    private int food;

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
        int gatheredFood = 0;

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
            switch (tree.type)
            {
                case ResourceType.fruitTree:
                    {
                        /*wood += tree.resourceAmount;*/
                        gatheredWood = tree.resourceAmount;
                        break;
                    }

                case ResourceType.leafTree:
                    {
                        /*wood += tree.resourceAmount;*/
                        gatheredWood = tree.resourceAmount;
                        break;
                    }

                case ResourceType.pineTree:
                    {
                        /*wood += tree.resourceAmount;*/
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
                case ResourceType.blueBerry:
                    {
                        food += blueBerryAmount;
                        break;
                    }

                case ResourceType.lingonBerry:
                    {
                        wood += lingonBerryAmount;
                        break;
                    }

                case ResourceType.apple:
                    {
                        wood += appleAmount;
                        break;
                    }
                case ResourceType.mushroom:
                    {
                        wood += mushroomAmount;
                        break;
                    }
                case ResourceType.venison:
                    {
                        wood += venisonAmount;
                        break;
                    }
            }*/
        }

        wood += gatheredWood;
        food += gatheredFood;

        InventoryCapacityFailsafe(gatheredWood, gatheredFood);

        currentInventory = (wood + food);

        print($"Food: {food}. Wood: {wood}");
    }

    void WhatResourceIsThis(GameObject obj)
    {

    }


    void InventoryCapacityFailsafe(int gatheredWood, int gatheredFood)
    {
        if (inventoryMaxCapacity - (wood + food) < 0)
        {
            int correction = inventoryMaxCapacity - (wood + food);

            if (gatheredWood > 0)
            {
                //correction is a negative value, therefore we "add" the negative value to subtract
                wood += correction;
            }
            else if (gatheredFood > 0)
            {
                //correction is a negative value, therefore we "add" the negative value to subtract
                food += correction;
            }

            print("Inventory is full");
        }
    }
}
