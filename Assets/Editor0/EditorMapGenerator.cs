using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this to run code from the editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorMapGenerator : MonoBehaviour
{

    private MapGenerator mapScript;

    public void GenerateTerrain()
    {
        // Get reference to the script
        mapScript = GetComponent<MapGenerator>();

        // Run it 
        mapScript.GenerateMap();
    }
    public void RandomiseTerrain()
    {
        // Get reference to the script
        mapScript = GetComponent<MapGenerator>();

        // Run it 
        mapScript.RandomiseMap();
    }


    //public void Spawner()
    //{
    //    // Get reference to the script
    //    spawnerScript = spawner.GetComponent<CritterSpawner>();

    //    // Run it 
    //    spawnerScript.Spawner(XNA);
    //}

    //public void Mutate()
    //{
    //    // Get reference to the script
    //    spawnerScript = spawner.GetComponent<CritterSpawner>();

    //    // Run it 
    //    spawnerScript.Mutate(XNA);
    //}

    //public void Reset()
    //{
    //    for (int i = 0; i < XNA.Length; i++)
    //    {
    //        XNA[i] = 1;
    //    }
    //}

}

// This runs the code in the editor
[CustomEditor(typeof(EditorMapGenerator))]
public class MapEditorButtons : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Reference to the EditorMapGenerator script
        EditorMapGenerator editorScripts = (EditorMapGenerator)target;

        // Add buttons to run the functions
        if (GUILayout.Button("GenerateTerrain"))
        {
            editorScripts.GenerateTerrain();
        }
        if (GUILayout.Button("RandomiseTerrain"))
        {
            editorScripts.RandomiseTerrain();
        }
        //if (GUILayout.Button("Mutate"))
        //{
        //    editorScripts.Mutate();
        //}
        //if (GUILayout.Button("Spawn"))
        //{
        //    editorScripts.Spawner();
        //}
        //if (GUILayout.Button("Reset"))
        //{
        //    editorScripts.Reset();
        //}
    }
}

