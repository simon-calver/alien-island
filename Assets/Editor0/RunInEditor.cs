using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this to run code from the editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RunInEditor : MonoBehaviour
{
  
    // Get reference to code to be run in the editor
    public GameObject spawner;
    private CritterSpawner spawnerScript;

    // Varibles to put in spawner script
    public int[] XNA;

    public void SpawnerStart()
    {
        // Get reference to the script
        spawnerScript = spawner.GetComponent<CritterSpawner>();

        // Run it 
        spawnerScript.Start();
    }

    public void Spawner()
    {
        // Get reference to the script
        spawnerScript = spawner.GetComponent<CritterSpawner>();

        // Run it 
        spawnerScript.Spawner(XNA);
    }

    public void Mutate()
    {
        // Get reference to the script
        spawnerScript = spawner.GetComponent<CritterSpawner>();

        // Run it 
        spawnerScript.Mutate(XNA);
    }

    public void Reset()
    {
        for (int i = 0; i < XNA.Length; i++)
        {
            XNA[i] = 1;
        }
    }

}

// This runs the code in the editor
[CustomEditor(typeof(RunInEditor))]
public class EditorButtons : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Reference to the RunInEditor script
        RunInEditor editorScripts = (RunInEditor)target;

        // Add buttons to run the functions
        if (GUILayout.Button("Start"))
        {
            editorScripts.SpawnerStart();
        }
        if (GUILayout.Button("Mutate"))
        {
            editorScripts.Mutate();
        }
        if (GUILayout.Button("Spawn"))
        {
            editorScripts.Spawner();
        }
        if (GUILayout.Button("Reset"))
        {
            editorScripts.Reset();
        }
    }
}

