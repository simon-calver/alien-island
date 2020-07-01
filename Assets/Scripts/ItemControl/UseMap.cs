using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMap : SwitchableItem
{
    public GameObject minimapCanvas;

    public override void OnEquipItem(GameObject HUD)
    {
        // Get reference to the canvas with the minimap attached and make sure it is active
        minimapCanvas = HUD.transform.Find("Minimap").gameObject;
        minimapCanvas.SetActive(true);
    }

    public override void OnUnequipItem(GameObject HUD)
    {
        // Get reference to the canvas with the minimap attached and make sure it is active
        minimapCanvas = HUD.transform.Find("Minimap").gameObject;
        minimapCanvas.SetActive(false);
    }

    public override void MainItemUse()
    {
        // Turn the map on and off, the appearance of the prefab needs to change and the map canvas turned off
        if (itemOn)
        {
            //offSound.Play();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            this.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            minimapCanvas.SetActive(true);
            itemOn = false;
        }
        else if (!itemOn)
        {
            //onSound.Play();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            this.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            minimapCanvas.SetActive(false);
            itemOn = true;
        }
    }
}
