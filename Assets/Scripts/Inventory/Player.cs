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

    // For keeping track of usable equipped items
    private bool rightArmEquipped;

    public Attribute[] attributes;

    private bool inventoryDisplayActive;

    // Store nearest interactable game object in the vicinity of the player, these should all have triggers on them
    private GameObject interactableObject;

    private void Start()
    {
        // Creates new ModifiableInt for each attribute, the value in the inspector needs
        // to be added. I think this is because there is a method that is added to the class 
        // that displays info in the editor: this can be removed...
        for (int i = 0; i < attributes.Length; i++)
        {
            int baseValue = attributes[i].value.BaseValue;
            attributes[i].SetParent(this);
            attributes[i].value.BaseValue = baseValue;
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            // The method used to update the inventory slots is added here, thats's what the delegate function in InventoryObject does
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
            equipment.GetSlots[i].OnBeforeEquipmentUpdate += OnRemoveItemEquipment;
            equipment.GetSlots[i].OnAfterEquipmentUpdate += OnAddItemEquipment;
        }

        // Make sure the inventory is not displayed initially
        inventoryDisplayActive = true;
        DisplayInventory();

        // Set what is displayed on the HUD initially
        HUDInitialDisplay();

        // Get reference to the player movement script, so values in it can be updated from this script
        // ITS PROBABLY A BETTER IDEA TO CHECK THAT THESE EXISTS ETC RATHER THAN USEING AN INDEX
        UpdateMovementSpeed();
    }

    public void UpdateMovementSpeed()
    {
        this.gameObject.GetComponent<PlayerMovement>().walkSpeed = attributes[0].value.ModifiedValue;
        this.gameObject.GetComponent<PlayerMovement>().runSpeed = attributes[1].value.ModifiedValue;
    }

    // Updates the inventory display and the player database when item is removed from slot
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

                // Update any scripts that use the attribute values
                UpdateMovementSpeed();

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

                // Update any scripts that use the attribute values
                UpdateMovementSpeed();

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }
    }


    // Updates the prefab in the player item slot when the item is removed
    public void OnRemoveItemEquipment(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:

                // The slot position corresponds to the part of the body this object is attached to
                Transform body_part_slot = this.transform.GetChild(_slot.slotPosition).Find("Item Slot");

                // Delete any existing prefabs in this slot after running the unequip method
                for (int i = 0; i < body_part_slot.transform.childCount; i++)
                {
                    body_part_slot.transform.GetChild(i).gameObject.GetComponentInChildren<UseItem>().OnUnequipItem(HUD);
                    Destroy(body_part_slot.transform.GetChild(i).gameObject);
                }

                // For now keep track of what is equipped like this
                if (_slot.slotPosition == 3)
                {
                    rightArmEquipped = false;
                }

                break;
            case InterfaceType.Chest:
                break;
            default:
                break;
        }

    }
    public void OnAddItemEquipment(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:

                // Check if this item can be dispayed on the player
                if (_slot.ItemObject.characterDisplay != null)
                {

                    // The slot position corresponds to the part of the body this object is attached to
                    Transform body_part_slot = this.transform.GetChild(_slot.slotPosition).Find("Item Slot");
                    //Transform body_part = this.transform.GetChild(_slot.slotPosition);

                    // Delete any existing prefabs in this slot
                    for (int i = 0; i < body_part_slot.transform.childCount; i++)
                    {
                        Destroy(body_part_slot.transform.GetChild(i).gameObject);
                    }

                    // Add the item prefab as child of the corresponding body part with the correct rotation and position
                    var obj = Instantiate(_slot.ItemObject.characterDisplay, body_part_slot.transform.position, body_part_slot.transform.rotation, transform);

                    // Set the parent of this and reset the rotation and position to zero (IS it neccessary to do this outside of the instantiate?)
                    obj.transform.transform.SetParent(body_part_slot.transform, false);
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localPosition = Vector3.zero;

                    if (_slot.slotPosition == 3)
                    {
                        rightArmEquipped = true;
                    }

                    // This function takes a gameobject as its argument with most derived classes do not use it,
                    // it seems silly to pass the argument to them all
                    InventorySlot equippedItem = equipment.GetSlots[_slot.slotPosition];
                    obj.GetComponentInChildren<UseItem>().OnEquipItem(HUD, equippedItem);
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

        // Use right click to interact with things
        if (Input.GetMouseButtonDown(0) & interactableObject)
        {
            // Check if the object is an item, i.e. it has the GroundItem script attached 
            if (interactableObject.GetComponent<GroundItem>())
            {
                // Add the new item to the inventory and delete the gameobject
                Item _item = new Item(interactableObject.GetComponent<GroundItem>().item);
                if (inventory.AddItem(_item, interactableObject.GetComponent<GroundItem>().amount))
                {
                    Destroy(interactableObject);
                    interactableObject = null;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) & rightArmEquipped)
        {
            this.transform.GetChild(3).GetComponentInChildren<UseItem>().MainItemUse();
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

    // Set which canvases ara initially displayed in the game using their names (so make sure you change this 
    // if the names are changed!)
    private void HUDInitialDisplay()
    {
        HUD.transform.Find("Minimap").gameObject.SetActive(false);
    }


    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
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