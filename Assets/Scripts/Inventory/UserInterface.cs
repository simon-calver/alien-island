﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public abstract class UserInterface : MonoBehaviour
{
    // The database containing the players items
    public InventoryObject inventory;

    // This is used to display the text stored in ItemObject.description 
    public GameObject textPopup;

    // Use this to lookup the inventory slot that the game object is in 
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            // 
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });

        // Set raycastTarget to false so the text doesn't block the mouse, this can also be done in the editor
        textPopup.GetComponentInChildren<TextMeshProUGUI>().raycastTarget = false;
        textPopup.GetComponentInChildren<Image>().raycastTarget = false;

        // Make the text box see-through
        textPopup.GetComponentInChildren<TextMeshProUGUI>().text = "";
        textPopup.GetComponentInChildren<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0f);
    }

    private void OnSlotUpdate(InventorySlot _slot)
    {
        // Updates the display in the inventory; item image, amount text and slider value
        if (_slot.item.Id >= 0)
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.uiDisplay; 
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            if (_slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>() != null)
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");

            // If this item has a charge stat this is displayed also
            if (_slot.ItemObject.chargable)
            {
                for (int i = 0; i < _slot.item.variableStats.Length; i++)
                {
                    if (_slot.item.variableStats[i].stat == VariableItemAttributes.Charge || _slot.item.variableStats[i].stat == VariableItemAttributes.Fuel)
                    {
                        _slot.slotDisplay.GetComponentInChildren<Slider>().value = (float)_slot.item.variableStats[i].value / 100f;
                        break;
                    }
                }
                SliderColourGradient(_slot.slotDisplay.transform.GetComponentInChildren<Slider>());
            }
            else
            {
                SliderClear(_slot.slotDisplay.transform.GetComponentInChildren<Slider>());
            }
            
        }
        else
        {
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            if (_slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>() != null)
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";

            SliderClear(_slot.slotDisplay.transform.GetComponentInChildren<Slider>());
        }
    }

    // Make a colour that transitions from red to green as the slider value is increased
    public void SliderColourGradient(Slider slider)
    {               
        Color fullColour = new Color(0.1f, 0.1f, 0.8f, 1f);
        Color emptyColour = new Color(0.1f, 0.1f, 0.2f, 1f);
        slider.targetGraphic.GetComponent<Image>().color = Color.Lerp(emptyColour, fullColour, slider.value);

        //Color newColour = new Color(1f - (slider.value / slider.maxValue),
        //                            slider.value / slider.maxValue,
        //                            0f, 1f);
        //slider.targetGraphic.GetComponent<Image>().color = newColour;
        slider.gameObject.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    // Makes the sprites attached to the slider transparent
    public void SliderClear(Slider slider)
    {
        slider.value = 0;
        Color newColour = new Color(1f, 1f, 1f, 0f);
        slider.targetGraphic.GetComponent<Image>().color = newColour;
        slider.gameObject.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    public abstract void CreateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        // Reference to the inventory slot the mouse is over
        MouseData.slotHoverOver = obj;

        // When mouse is over the inventory slot, if it has an item in it, display the item description. The value in the 
        // dictionary is the inventory slot associated with this game object. To access the item description we need 
        // to get the ItemObject in the slot
        if (slotsOnInterface[obj].item.Id >= 0)
        {
            // Move the decription text to the position of the slot, it may be neccessary to add an offset
            textPopup.GetComponent<RectTransform>().position = obj.GetComponent<RectTransform>().position;

            // Update the text display and background colour
            textPopup.GetComponentInChildren<TextMeshProUGUI>().text = slotsOnInterface[obj].ItemObject.data.Name+": "+slotsOnInterface[obj].ItemObject.description;
            textPopup.GetComponentInChildren<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
    }
       
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoverOver = null;

        // Remove the description text from the popup and make the background transparent
        textPopup.GetComponentInChildren<TextMeshProUGUI>().text = "";
        textPopup.GetComponentInChildren<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0f);
    }

    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }

    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if(slotsOnInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);

            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay; //inventory.database.GetItem[slotsOnInterface[obj].item.Id].uiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;
    }

    // Updates the positions of the item in the inventory when mouse is released
    public void OnDragEnd(GameObject obj)
    {
        // Remove the object showing what is being moved
        Destroy(MouseData.tempItemBeingDragged);

        // If the object is released outside of the inventory show a pop-up "Do you want to delete"
        // If it is over an inventroy slot check if the things can be combined
        // The item in the inventory is deleted if it is released outside of the inventory screens
        if(MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }
        if (MouseData.slotHoverOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoverOver];

            // If the object being moved is consumable check if the slot it is dragged to has an item with the same stats
            if (slotsOnInterface[obj].ItemObject.type == ItemType.Consumable &&  mouseHoverSlotData.ItemObject != null)
            {
                inventory.CombineItems(slotsOnInterface[obj], mouseHoverSlotData);
            }
            else
            {
                inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
            }
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        
    }
}

public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    public static GameObject slotHoverOver;
}

//public static class ExtensionMethods
//{
//    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
//    {
//        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
//        {
//            if (_slot.Value.item.Id >= 0)
//            {
//                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;//inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
//                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);

//                // Not all slots have text mesh pro components, this checks that they do before changing them 
//                if (_slot.Key.GetComponentInChildren<TextMeshProUGUI>() != null)
//                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
//            }
//            else
//            {
//                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
//                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);

//                if (_slot.Key.GetComponentInChildren<TextMeshProUGUI>() != null)
//                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
//            }
//        }
//    }
//}
