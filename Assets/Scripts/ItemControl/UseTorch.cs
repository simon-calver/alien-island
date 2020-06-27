using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTorch : SwitchableItem
{
    UnityEngine.Experimental.Rendering.Universal.Light2D light;

    public AudioSource onSound;
    public AudioSource offSound;

    public override void MainItemUse()
    {
        light = this.gameObject.transform.GetChild(0).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();

        // Turn the light on and off using setActive on the light component
        if (itemOn)
        {
            offSound.Play();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            itemOn = false;
        }
        else if (!itemOn)
        {
            onSound.Play();
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            itemOn = true;
        }
        
        
        
    }
}
