using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTorch : SwitchableItem
{
    UnityEngine.Experimental.Rendering.Universal.Light2D light;

    public AudioSource onSound;
    public AudioSource offSound;

    private Flicker flicker;

    public override void MainItemUse()
    {
        light = this.gameObject.transform.GetChild(0).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        flicker = light.GetComponent<Flicker>();

        // Turn the light on and off using setActive on the light component
        if (itemOn)
        {
            offSound.Play();
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
            itemOn = false;
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
}
