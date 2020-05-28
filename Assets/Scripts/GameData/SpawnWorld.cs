using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PlayerInfo;

public class SpawnWorld : MonoBehaviour
{
    public 
    // The map generator should be attached to the same gameobject as this script
    //public GameObject mapGenerator;

    // Start is called before the first frame update
    void Start()
    {
        // Load the world data 
        WorldInfo loadedData = DataSaver.loadData<WorldInfo>("newWorldData");

        // Set the parameters in the map generator
        GetComponent<MapGenerator>().heightOffset = loadedData.heightOffset;
        GetComponent<MapGenerator>().biomeOffset = loadedData.biomeOffset;

        // Generate the map
        GetComponent<MapGenerator>().GenerateMap();

        // Spawn the player prefab and set its properties

        //mapGenerator.GetComponent<MapGenerator>().heightOffset = loadedData.heightOffset;

        //mapGenerator.GetComponent<MapGenerator>().GenerateMap();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
