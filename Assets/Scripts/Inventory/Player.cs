using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Canvas gameobjects in the scene
    public GameObject inventoryDisplay;
    public GameObject HUD;

    public InventoryObject inventory;
    public InventoryObject equipment;

    public GameObject leftArm;
    public GameObject rightArm;

    public Attribute[] attributes;

    private bool inventoryDisplayActive;

    // Store nearest interactable game object in the vicinity of the player, these should all have triggers on them
    private GameObject interactableObject;

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }

        // Make sure the inventory is not displayed initially
        inventoryDisplayActive = true;
        DisplayInventory();
    }

    public void OnRemoveItem(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }

                // The slot position corresponds to the part of the body this object is attached to
                Transform body_part = this.transform.GetChild(_slot.slotPosition);

                // Delete any existing prefabs in this slot
                for (int i = 0; i < body_part.transform.childCount; i++)
                {
                    Destroy(body_part.transform.GetChild(i).gameObject);
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }

    } 
    public void OnAddItem(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)                        
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);                        
                    }
                }

                // Check if this item can be dispayed on the player
                if (_slot.ItemObject.characterDisplay != null)
                {

                    // The slot position corresponds to the part of the body this object is attached to
                    Transform body_part = this.transform.GetChild(_slot.slotPosition);

                    // Delete any existing prefabs in this slot
                    for (int i = 0; i < body_part.transform.childCount; i++)
                    {
                        Destroy(body_part.transform.GetChild(i).gameObject);
                    }

                    // Add the item prefab as child of the corresponding body part with the correct rotation and position
                    var obj = Instantiate(_slot.ItemObject.characterDisplay, body_part.transform.position, body_part.transform.rotation, transform);

                    // Set the parent of this and reset the rotation to zero (IS it neccessary to do this outside of the instantiate?)
                    obj.transform.transform.SetParent(body_part.transform, false);
                    obj.transform.localRotation = Quaternion.identity;
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        interactableObject = other.gameObject;
        // 
        //var item = other.GetComponent<GroundItem>();
        //if (item)
        //{
        //    // Highlight the item
        //    Debug.Log(other.transform.position);


        //    Item _item = new Item(item.item);
        //    if(inventory.AddItem(_item, 1))
        //    {
        //        Destroy(other.gameObject);
        //    }



        //    //inventory.AddItem(new Item(item.item), 1);
        //    // 
        //}
    }

    void OnTriggerExit2D(Collider2D other)
    {
        interactableObject = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inventory.Load();
            equipment.Load();
        }

        // Open/close the inventory screen
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayInventory();
        }

        if (Input.GetMouseButtonDown(0) & interactableObject)
        {
            // Check if the object is an item, i.e. it has the GroundItem script attached 
            if (interactableObject.GetComponent<GroundItem>())
            {
                // Add the new item to the inventory and delete the gameobject
                Item _item = new Item(interactableObject.GetComponent<GroundItem>().item);
                if (inventory.AddItem(_item, 1))
                {
                    Destroy(interactableObject);
                    interactableObject = null;
                }
            }
        }

    }

    // Enable and disable the inventory screen, this also probably needs to pause the game
    private void DisplayInventory()
    {
        if (inventoryDisplayActive)
        {
            // Disable the game object and update the boolean
            inventoryDisplay.SetActive(false);
            inventoryDisplayActive = false;
        }
        else
        {
            // Enable the game object and update the boolean
            inventoryDisplay.SetActive(true);
            inventoryDisplayActive = true; 
        }

        // Update all the slots to display the items
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].UpdateSlot(inventory.GetSlots[i].item, inventory.GetSlots[i].amount);
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now", attribute.value.ModifiedValue));
    }

    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public Player parent;
    public Attributes type;
    public ModifiableInt value;

    public void SetParent(Player _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}