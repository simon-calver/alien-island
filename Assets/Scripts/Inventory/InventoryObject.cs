using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;

public enum InterfaceType
{
    Inventory,
    Equipment,
    Chest
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject 
{
    public string savePath;
    public ItemDatabaseObject database;
    public InterfaceType type;
    public Inventory Container;
    public InventorySlot[] GetSlots { get { return Container.Slots; } }

    public bool AddItem(Item _item, int _amount)
    {
        if (EmptySlotCount <= 0)
            return false;
        InventorySlot slot = FindItemOnInventory(_item);
        if(!database.ItemObjects[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.Id <= -1)
                    counter++;                    
            }
            return counter;
        }
    }

    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id == _item.Id)
            {
                return GetSlots[i];
            }
        }
        return null;
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item.Id <= -1)
            {
                GetSlots[i].UpdateSlot(_item, _amount);
                return GetSlots[i];
            }
        }
        // Set full inventory function here
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }

    }

    // There needs to be a pop-up to choose between combine and swap
    public void CombineItems(InventorySlot item1, InventorySlot item2)
    {
        // Count how many changes are made, if nothing is changed the items are swapped otherwise item1 has its amount reduced by 1
        int updatesCount = 0;

        //
        // The items in the database needs to be found so the values on this can be updated, the items passed to the function are the 
        // diplay prefabs 
        //for (int i = 0; i < item1.ItemObject.data.variableStats.Length; i++)
        //{
        //    for (int j = 0; j < item2.ItemObject.data.variableStats.Length; j++)
        //    {
        //    //    Debug.Log(slotsOnInterface[obj].item.variableStats[0]);//.data.variableStats[0]);// parent.inventory.database.ItemObjects[slotsOnInterface[obj].item.Id].uiDisplay);

        //        if (item2.ItemObject.data.variableStats[j].stat == item1.ItemObject.data.variableStats[i].stat)
        //        {
        //            item2.ItemObject.data.variableStats[j].AddValue(item1.ItemObject.data.variableStats[i].value); 
        //            updatesCount += 1;
        //        }
        //    }
        //}
        for (int i = 0; i < item1.item.variableStats.Length; i++)
        {
            for (int j = 0; j < item2.item.variableStats.Length; j++)
            {
                Debug.Log(item1.item.variableStats.Length);//.data.variableStats[0]);// parent.inventory.database.ItemObjects[slotsOnInterface[obj].item.Id].uiDisplay);
                Debug.Log(item1.item.variableStats[i].value);
                if (item2.item.variableStats[j].stat == item1.item.variableStats[i].stat)
                {
                    item2.item.variableStats[j].AddValue(item1.item.variableStats[i].value);
                    updatesCount += 1;
                }
            }
        }
        // Remove an item if it has been used otherwise swap their positions
        if (updatesCount > 0)
        {
            item1.RemoveAmount(1); 
        }
        else
        {
            SwapItems(item1, item2);
        }
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if(GetSlots[i].item == _item)
            {
                GetSlots[i].UpdateSlot(null, 0);
            }
        }
    }

    public void Save()
    {
        //Debug.Log(Application.persistentDataPath);
        //Debug.Log(savePath);
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < GetSlots.Length; i++)
            {
                GetSlots[i].UpdateSlot(newContainer.Slots[i].item, newContainer.Slots[i].amount);
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();// = new Inventory();
    }

}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[24];
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot _slot);



[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    public Item item;
    public int amount;

    // This is used to find the position of this slot from the user interface 
    public int slotPosition;

    [System.NonSerialized]
    public GameObject slotDisplay;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;

    public ItemObject ItemObject
    {
        get
        {
            if(item.Id >= 0)
            {
                return parent.inventory.database.ItemObjects[item.Id];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }
    public InventorySlot(Item _item,  int _amount)
    {
        UpdateSlot(_item, _amount);
    }
    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);         
    }
    public void RemoveAmount(int value)
    {
        if (amount > 1)
        {
            UpdateSlot(item, amount -= value);
        }
        else
        {
            RemoveItem();
        }
        
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        if (OnBeforeUpdate != null)
            OnBeforeUpdate.Invoke(this);
        item = _item;
        amount = _amount;
        if (OnAfterUpdate != null)
            OnAfterUpdate.Invoke(this);
    }

    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)        
            return true;
        
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }
}
