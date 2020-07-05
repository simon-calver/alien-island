using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The item types what is enum?
public enum ItemType
{
    Headgear,
    Equipment,
    Bodywear,
    Legwear,
    Footwear,
    Consumable
}

public enum Attributes
{
    Stamina,
    WalkSpeed, 
    RunSpeed,
    Strength
}

public enum FixedItemAttributes
{
    Weight,
    Strength
}

public enum VariableItemAttributes
{
    Charge,
    Fuel
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/Item")]
public class ItemObject : ScriptableObject
{
    public Sprite uiDisplay;
    public GameObject characterDisplay;
    public bool stackable;
    public bool chargable;
    public ItemType type;
    [TextArea(15,20)]
    public string description;
    
    public Item data = new Item();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public FixedItemStats[] fixedStats;
    public VariableItemStats[] variableStats;

    public Item()
    {
        Name = "";
        Id = -1;
    }

    // This creates the version of the item that goes in the inventory
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;

        // The buff types are set in the item object in the editor, the values are chosen randomly
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attribute = item.data.buffs[i].attribute
            };
        }
        // The fixed stats do not change, they are set in the editor
        fixedStats = new FixedItemStats[item.data.fixedStats.Length];
        for (int i = 0; i < fixedStats.Length; i++)
        {
            fixedStats[i] = new FixedItemStats(item.data.fixedStats[i].value)
            {
                stat = item.data.fixedStats[i].stat
            };
        }
        // The variable stats do change, the initial value on picking up the item is set in the editor
        variableStats = new VariableItemStats[item.data.variableStats.Length];
        for (int i = 0; i < fixedStats.Length; i++)
        {
            variableStats[i] = new VariableItemStats(item.data.variableStats[i].min, item.data.variableStats[i].max, item.data.variableStats[i].value)
            {
                stat = item.data.variableStats[i].stat
            };
        }
    }

    //// Generate an array of the variableStats this item has 
    //public VariableItemAttributes[] VariableStatsArray()
    //{
    //    VariableItemAttributes[] statsArray = new VariableItemAttributes[variableStats.Length];
    //    for (int i = 0; i < variableStats.Length; i++)
    //        statsArray[i] = variableStats[i].stat;

    //    return statsArray;
    //}
}

// Class for storing item buffs (values passed to the player)
[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;

    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}

// Class for storing item stats
[System.Serializable]
public class FixedItemStats
{
    public FixedItemAttributes stat;
    public float value;

    public FixedItemStats(float _value)
    {
        value = _value;
    }
}

[System.Serializable]
public class VariableItemStats
{
    public VariableItemAttributes stat;
    public int value;
    public int min;
    public int max;

    public VariableItemStats(int _min, int _max, int _value)
    {
        min = _min;
        max = _max; 
        value = _value;
    }

    public void AddValue(int valueToAdd)
    {
        value = Mathf.Clamp(value + valueToAdd, min, max);
    }

    public void RemoveValue(int valueToRemove)
    {
        value = Mathf.Clamp(value - valueToRemove, min, max);
    }
}