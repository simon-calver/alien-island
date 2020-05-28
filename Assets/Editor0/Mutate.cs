using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this to run code from the editor
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Mutate : MonoBehaviour
{

    // Put the prefab to spawn here
    public GameObject Critter;
    private GameObject currentCritter;

    // 
    public Vector3 position;

    public int[] XNA;
    private int XNA_length;
    private int XNA_index;

    public float[] scales = new float[] { 0.5f, 1f, 1.5f, 2f };

    // Start is called before the first frame update
    void Start()
    {
        XNA = new int[41]; 
    }

    // Spawn the gameobject
    public void Spawner(Vector3 position)
    {
        scales = new float[] {0f, 0.5f, 1f, 1.5f, 2f };
        // If a critter has already been spawned, delete it
        //if (currentCritter == null)
        //{
        //    Debug.Log("Wa");
        //    DestroyImmediate(currentCritter);
        //}
        // Get the length of the XNA array
        XNA_length = XNA.Length;
        XNA_index = 0;

        // Instantiate at position and zero rotation
        GameObject newCritter = Instantiate(Critter, position, Quaternion.identity);
        Transform newCritterTransform = newCritter.transform;
        currentCritter = newCritter;

        // Get reference to the root bone 
        //Transform rootBone = newCritter.transform.Find("root");

        foreach (Transform g in newCritterTransform.GetComponentsInChildren<Transform>())
        {
            if (g.tag == "Bone" & XNA_index < XNA_length)
            {
                g.localScale *= scales[XNA[XNA_index]];
                XNA_index += 1;
            }
            //Debug.Log(bones_index);
            //bones[bones_index] = g;
            //bones_index++;

        }

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

// This runs the code in the editor
[CustomEditor(typeof(Mutate))]
public class MutateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Reference to the BiomeGenerator script
        Mutate spawner = (Mutate)target;

        // Add buttons to run the functions
        if (GUILayout.Button("Spawn"))
        {
            spawner.Spawner(spawner.position);
        }

    }
}

