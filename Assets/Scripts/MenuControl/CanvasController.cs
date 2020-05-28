using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Array all canvases in the scene
    public GameObject[] menuCanvas;
    public GameObject mapCamera;

    void Start()
    {
        // The first canvas in the list is assumed to be the default, the rest are made inactive
        EnableCanvas(0);
    }

    public void EnableCanvas(int active_ind)
    {
        // Activate the canvas at position active_ind in menuCanvas and deactivate the rest
        for (int canvas_ind = 0; canvas_ind < menuCanvas.Length; canvas_ind++)
        {
            if (canvas_ind != active_ind)
            {
                menuCanvas[canvas_ind].SetActive(false);
            }
            else
            {
                menuCanvas[canvas_ind].SetActive(true);
            }
            
        }
    }

    public void FitCameraToMap(int mapWidth, int mapHeight)
    {
        // Position camera in the centre of the map so it can see the whole thing
        mapCamera.transform.localPosition = new Vector3((float)mapWidth/2 + 19.5f, (float)mapHeight / 2 + 19.5f, -10f);
        mapCamera.GetComponent<Camera>().orthographicSize = (float)Mathf.Max(mapWidth, mapHeight) / 2 -0.5f;
    }
}
