using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : MonoBehaviour
{
    public virtual void OnEquipItem(GameObject _minimapCanvas, InventorySlot _slot)
    {
    }

    public virtual void OnUnequipItem(GameObject _minimapCanvas)
    {
    }

    public virtual void MainItemUse()
    {
    }

    public virtual void ItemFail()
    {
    }
}
