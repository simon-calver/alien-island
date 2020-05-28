using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterSpawner : MonoBehaviour
{

    // Put the prefab to spawn here
    public GameObject Critter;

    // Variables for the position and rotation of the spawned object
    private Vector2 newPos;
    private float angleToSpawn;
    private float radiusToSpawn;
    private Vector3 forwardDirection = new Vector3(0f, 0f, 1f);

    // Possible colours for the sprites
    private readonly Color[] spriteColours = { new Color(0.27f, 0.18f, 0.18f, 1),
                                               new Color(0.18f, 0.37f, 0.23f, 1),
                                               new Color(0.61f, 0.66f, 0.32f, 1),
                                               new Color(0.18f, 0.14f, 0.14f, 1),
                                               new Color(0.42f, 0.34f, 0.12f, 1) };
    private readonly float[] scales = new float[] { 0.25f, 1.0f, 0.5f, 1.5f, 2f };

    private int XNA_length;
    private int XNA_index;

    private Transform[] leftParts;
    private int leftPartsLength;
    private int leftPartsIndex;
    private Transform[] rightParts;
    private int rightPartsLength;
    private int rightPartsIndex;

    private float[] leftBodyScales;
    private float[] rightBodyScales;

    private int leftCount;
    private int rightCount;

    // Start is called before the first frame update
    public void Start()
    {
        // Count how manybody part shave the "Left" tag
        leftPartsLength = 0;
        foreach (Transform childTransform in Critter.transform.GetComponentsInChildren<Transform>())
        {
            if (childTransform.tag == "Left")
            {
                leftPartsLength += 1;
            }
            if (childTransform.tag == "Right")
            {
                rightPartsLength += 1;
            }
        }
        leftParts = new Transform[leftPartsLength];
        rightParts = new Transform[rightPartsLength];

        leftBodyScales = new float[leftPartsLength];
        rightBodyScales = new float[rightPartsLength];

        //leftPartsIndex = 0;
        //foreach (Transform childTransform in Critter.transform.GetComponentsInChildren<Transform>())
        //{
        //    if (childTransform.tag == "Left")
        //    {
        //        leftParts[leftPartsIndex] = childTransform;
        //        leftPartsIndex += 1;
        //    }
        //}

        //// Count how manybody part shave the "Right" tag
        //rightPartsLength = 0;
        //foreach (Transform childTransform in Critter.transform.GetComponentsInChildren<Transform>())
        //{
        //    if (childTransform.tag == "Right")
        //    {
        //        rightPartsLength += 1;
        //    }
        //}

        //// Now make a new transform array with these in 
        //rightParts = new Transform[rightPartsLength];
        //rightPartsIndex = 0;
        //foreach (Transform childTransform in Critter.transform.GetComponentsInChildren<Transform>())
        //{
        //    if (childTransform.tag == "Right")
        //    {
        //        rightParts[rightPartsIndex] = childTransform;
        //        rightPartsIndex += 1;
        //    }
        //}

        // Get an array of body parts   
        //foreach (Transform childTransform in newCritterTransform.GetComponentsInChildren<Transform>())
        //{
        //    if (childTransform.tag == "Left" & XNA_index < XNA_length)
        //    {
        //        leftParts = 1
        //        //childTransform.localScale *= scales[XNA[XNA_index]];
        //        //XNA_index += 1;
        //        //Debug.Log(childTransform.name);
        //    }
        //}
    }

    // Spawn the gameobject using the XNA to set the properties of the prefab
    public void Spawner(int[] XNA)
    {
        // Get the length of the XNA array
        XNA_length = XNA.Length;
        XNA_index = 0;

        leftCount = 0;
        rightCount = 0;

        // The critters are spawned within an anulus centered on the position of the gameObject
        // this script is attached to 
        angleToSpawn = Random.Range(-Mathf.PI, Mathf.PI);
        radiusToSpawn = Random.Range(20f, 40f);
        newPos = new Vector2(transform.position.x + radiusToSpawn * Mathf.Cos(angleToSpawn), transform.position.x + radiusToSpawn * Mathf.Sin(angleToSpawn));

        // Instantiate at newPos with a random rotation 
        GameObject newCritter = Instantiate(Critter, newPos, Quaternion.Euler(180f * Random.Range(-1f, 1f) * forwardDirection));
        Transform newCritterTransform = newCritter.transform;

        //for (int i = 0; i < leftPartsLength; i++ ) {
        //    leftParts[i].localScale *= scales[XNA[i]];
        //    rightParts[i].localScale *= scales[XNA[i]];
        //}
        //Debug.Log(leftParts.Length);

        //poo = XNAToBody(XNA);

        XNAToBody(XNA);

        // Iterate through all children of the transform and modify ones with the "Bone" label
        //foreach (Transform childTransform in newCritterTransform.GetComponentsInChildren<Transform>())
        //{
        //    if (childTransform.tag == "Left")
        //    {
        //        childTransform.localScale *= leftBodyScales[leftCount];
        //        leftCount += 1;
        //        //Debug.Log(childTransform.name);
        //    }
        //    if (childTransform.tag == "Right")
        //    {
        //        childTransform.localScale *= rightBodyScales[rightCount];
        //        rightCount += 1;
        //        //Debug.Log(childTransform.name);
        //    }
        //    // If there is a sprite in efwhfg

        //    //if (childTransform.GetComponent<SpriteRenderer>() != null & XNA_index < XNA_length)
        //    //{
        //    //    childTransform.GetComponent<SpriteRenderer>().color = spriteColours[XNA[XNA_index]];
        //    //    XNA_index += 1;
        //    //}

        //    //Debug.Log(bones_index);
        //    //bones[bones_index] = g;
        //    //bones_index++;

        //}

        // Iterate through all children of the transform and modify ones with the "Bone" label
        foreach (Transform childTransform in newCritterTransform.GetComponentsInChildren<Transform>())
        {
            if (childTransform.tag == "Bone" & XNA_index < XNA_length)
            {
                childTransform.localScale *= scales[XNA[XNA_index]];
                XNA_index += 1;
                //Debug.Log(childTransform.name);
            }
            // If there is a sprite in efwhfg

            if (childTransform.GetComponent<SpriteRenderer>() != null & XNA_index < XNA_length)
            {
                childTransform.GetComponent<SpriteRenderer>().color = spriteColours[XNA[XNA_index]];
                XNA_index += 1;
            }

            //Debug.Log(bones_index);
            //bones[bones_index] = g;
            //bones_index++;

        }

        //// Get reference to the root bone 
        //Transform rootBone = newCritter.transform.Find("root");


        //foreach (Transform g in newCritterTransform.GetComponentsInChildren<Transform>())
        //{
        //    if (g.tag == "Bone" & XNA_index < XNA_length)
        //    {
        //        g.localScale *= scales[XNA[XNA_index]];
        //        XNA_index += 1;
        //    }
        //    //Debug.Log(bones_index);
        //    //bones[bones_index] = g;
        //    //bones_index++;

        //    //}






        //Mathf.Cos(-Mathf.PI / 180f * forwardDirection.z));

        //scales = new float[] { 0f, 0.5f, 1f, 1.5f, 2f };
        //// If a critter has already been spawned, delete it
        ////if (currentCritter == null)
        ////{
        ////    Debug.Log("Wa");
        ////    DestroyImmediate(currentCritter);
        ////}
        //// Get the length of the XNA array
        //XNA_length = XNA.Length;
        //XNA_index = 0;






        //  A random rotation between -pi and pi is used for the new direction, since thi is 2D only the z component needs to be changed

        // forwardDirection.z = Random.Range(-1, 1);

        //  This is converted to a quaternion(in degrees) for use in the Lerp function

        //rotation = Quaternion.Euler(180f * Random.Range(-1, 1));



    }

    // Mutate the genetic information
    public void Mutate(int[] XNA)
    {

        // Select random index
        int mutationLoc = Random.Range(0, XNA.Length);

        // Mutate it 
        XNA[mutationLoc] = Random.Range(0, 5);

    }

    // Transform XNA into array of scales
    public void XNAToBody(int[] XNA)
    {
        for (int i = 0; i < leftPartsLength; i++)
        {
            leftBodyScales[i] = scales[XNA[i]];
            rightBodyScales[i] = scales[XNA[i]];
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
