using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string type;
    public string description;
    public Sprite icon;
    public bool pickedUp;

    [HideInInspector]
    public bool equipped;

    [HideInInspector]
    public GameObject heldItem;

    [HideInInspector]
    public GameObject itemManager;

    public bool playersItem;

    public void Start()
    {
        itemManager = GameObject.FindWithTag("ItemManager");

        if (!playersItem)
        {
            int allItems = itemManager.transform.childCount;
            for (int i=0; i<allItems; i++)
            {
                if(itemManager.transform.GetChild(i).gameObject.GetComponent<Item>().ID == ID)
                {
                    heldItem = itemManager.transform.GetChild(i).gameObject;
                }
            }
        }
    }

    public void Update()
    {
        if (equipped)
        {

            if (Input.GetKey(KeyCode.G))
                equipped = false;

            if (equipped == false)
                this.gameObject.SetActive(true);
        }
    }


    public void ItemUsage()
    {
        if(type == "Light")
        {
            heldItem.SetActive(true);
            heldItem.GetComponent<Item>().equipped = true;
        }
    }
}
