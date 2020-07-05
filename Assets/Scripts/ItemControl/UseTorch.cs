using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTorch : SwitchableItem
{
    UnityEngine.Experimental.Rendering.Universal.Light2D light;

    public AudioSource onSound;
    public AudioSource offSound;
    public AudioSource failSound;

    public float maxIntensity;
    public float minIntensity;

    private Flicker flicker;

    float delay = 1;
    float delayedtime;

    private float itemCharge;

    InventorySlot inventorySlot;

    public override void OnEquipItem(GameObject HUD, InventorySlot _slot)
    {
        inventorySlot = _slot;
        light = this.gameObject.transform.GetChild(0).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        itemCharge = inventorySlot.item.variableStats[0].value;

        flicker = light.GetComponent<Flicker>();
        if (itemCharge >= 10)
        {
            if (flicker != null)
                flicker._baseIntensity = maxIntensity;
            else
                light.intensity = maxIntensity;
        }
        else if (itemCharge < 10 && itemCharge > 0)
        {
            
            if (flicker != null)
                flicker._baseIntensity = maxIntensity * itemCharge / 10;
            else
                light.intensity = maxIntensity * itemCharge / 10;

        }
        else
        {
            ItemFail();
        }
    }

    public override void MainItemUse()
    {
        if (itemCharge > 0)
        {
            // Turn the light on and off using setActive on the light component
            if (itemOn)
            {
                offSound.Play();
                for (int i = 0; i < this.gameObject.transform.childCount; i++)
                {
                    this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
                itemOn = false;
                if (flicker != null)
                    flicker._flickering = false;
            }
            else if (!itemOn)
            {
                onSound.Play();
                for (int i = 0; i < this.gameObject.transform.childCount; i++)
                {
                    this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
                }
                itemOn = true;
            }
        }
        else
        {
            ItemFail();
        }
    }

    public override void ItemFail()
    {
        itemOn = false;
        failSound.Play();
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        itemOn = false;
        if (flicker != null)
            flicker._flickering = false;
    }

    public void Update()
    {
        if (Time.time > delayedtime && itemOn)
        {
            inventorySlot.item.variableStats[0].RemoveValue(1);
            itemCharge = inventorySlot.item.variableStats[0].value;
            if (itemCharge >= 10)
            {
                if (flicker != null)
                    flicker._baseIntensity = maxIntensity;
                else
                    light.intensity = maxIntensity;
            }
            else if (itemCharge < 10 && itemCharge > 0)
            {
                if (flicker != null)
                    flicker._baseIntensity = maxIntensity * itemCharge / 10;
                else
                    light.intensity = maxIntensity * itemCharge / 10;
            }
            else
            {
                ItemFail();
            }
            inventorySlot.UpdateSlot(inventorySlot.item, inventorySlot.amount);
            delayedtime = Time.time + delay;
        }
    }
}
