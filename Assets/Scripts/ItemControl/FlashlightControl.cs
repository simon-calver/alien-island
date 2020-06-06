using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class FlashlightControl : MonoBehaviour
{
    public float lightIntensity;
    bool lightOn;
    UnityEngine.Experimental.Rendering.Universal.Light2D light;

    // Start is called before the first frame update
    void Start()
    {
        light = this.gameObject.transform.GetChild(0).GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        light.intensity = lightIntensity;
        lightOn = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown("space") && lightOn)
        {
            light.intensity = 0;
            lightOn = false;
        }
        if (Input.GetKeyDown("space") && !lightOn)
        {
            light.intensity = lightIntensity;
            lightOn = true;
        } 
    }
}
