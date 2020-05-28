using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CritterMovement : MonoBehaviour
{
    // Transforms in range 

    // on entering thing to avoid stop, step back, wander

    Animator playerAnimator;



    // Variables to access the transforms of this gameobject
    Rigidbody2D body;
    Transform critterTransform;
    public Transform targetTransform;

    // Get reference to the biome map 
    public GameObject mapGenerator;

    // Transforms that this can see, set a limit on how may things this can see
    public Transform[] inSight = new Transform[5];

    // Parameters
    public float moveSpeed = 2;
    public float pauseTime = 0f;
    public float moveTime = 0f;
    public float fleeTime = 1f;
    public float avoidTime = 1f;

    public int[] tilesToAvoid;
    //public var numbers = new List<int>();

    //private Vector2 critterPosition;

    // boolean variables to determine what movement should be performed
    public bool wander = true;
    public bool flee = false;
    public bool idle = false;
    public bool herd = false;
    public bool avoid = false;

    bool persue = false;

    // When flee is true make this true so there is only one flee call
    bool fleeing = false;
    float smooth = 5.0f;

    public float elapsed = 0f;
    private float currentSpeed;

    // Vector and quaternion for storing the direction the sprite is currently facing
    Vector3 forwardDirection = new Vector3(0, 0, 0);
    Quaternion rotation;// = new Quaternion.Euler(180f / Mathf.PI * forwardDirection);

    Vector2Int targetPosition; // = new Vector2Int((int)critterTransform.position.x, (int)critterTransform.position.y);
    int tileType;


    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the transform and rigid body attached to this object
        body = GetComponent<Rigidbody2D>();
        critterTransform = GetComponent<Transform>();

        playerAnimator = GetComponent<Animator>();

        currentSpeed = moveSpeed;

    }

    // Update is called once per frame
    void Update()
    {


        // Position of target tile
        targetPosition = new Vector2Int((int)critterTransform.position.x, (int)critterTransform.position.y);
        //tileType = mapGenerator.GetComponent<BiomeGenerator>().biomeMap[targetPosition.x, targetPosition.y];
        //Debug.Log(tileType);

        if (tilesToAvoid.Contains(tileType))//  == 1 | tileType == 0)
        {
            elapsed = 0f;
            avoid = true;
            wander = false;
            idle = false;
        }
        if (!tilesToAvoid.Contains(tileType) & avoid & elapsed >= avoidTime) //tileType != 1 & tileType != 0 
        {
            elapsed = 0f;
            avoid = false;
            wander = true;
            idle = false;
            elapsed = moveTime;
        }
        if (elapsed == 0 & flee & !fleeing)
        {
            // Find the new forward direction 
            forwardDirection.z = Mathf.PI/2 - Mathf.Atan2(critterTransform.position.y - targetTransform.position.y, critterTransform.position.x - targetTransform.position.x);
            // += Mathf.PI + Random.Range(-Mathf.PI/4, Mathf.PI/4); // //Debug.Log(forwardDirection.z);

            rotation = Quaternion.Euler(180f / Mathf.PI * forwardDirection);
            fleeing = true;
            //elapsed = 0f;
            //wander = true;
            //idle = false;
            //flee = false;

           
        }
        if (elapsed >= fleeTime & flee)
        {
            flee = false;
            fleeing = false;
            idle = true;
        }
        // Choose random direction to move in 
        if (elapsed >= moveTime & wander)
        {           
            // A random rotation between -pi and pi is used for the new direction, since thi is 2D only the z component needs to be changed
            forwardDirection.z = Random.Range(-Mathf.PI, Mathf.PI);

            // This is converted to a quaternion (in degrees) for use in the Lerp function 
            rotation = Quaternion.Euler(180f / Mathf.PI * forwardDirection);

            // Reset the time counter
            elapsed = 0f;  

            // Switch the current movement type 
            wander = false;
            idle = true;

        }
        // Stop moving 
        if (elapsed >= pauseTime & idle)
        {
            wander = true;
            idle = false;

            // Reset the time counter
            elapsed = 0f; 
        }

        // Time elapsed since movement type has changed
        elapsed += Time.deltaTime;

    }

    void FixedUpdate()
    {

        if (avoid)
        {
            Avoid();
        }
        else if (wander)
        {
            Wander();
        }
        else if (idle)
        {
            // Do nothing
            Idle();
        }
        else if (flee)
        {
            Flee(targetTransform);
        }
        else if (herd)
        {
            Herd();
        }

        // Update the speed in the animator 
        playerAnimator.SetFloat("Speed", currentSpeed);


    }

    // Choose target to move towards
    public void ChooseTarget()
    {


    }

    // Choose target to move towards
    public void Avoid()
    {
        currentSpeed = -0.5f * moveSpeed;
        body.velocity = new Vector2(-0.5f*moveSpeed * Mathf.Sin(-forwardDirection.z), -0.5f*moveSpeed * Mathf.Cos(-forwardDirection.z));    
    }

    // Move towards current target
    public void MoveTowards()
    {
        currentSpeed = moveSpeed;

        // Add velocity to the rigid body in the direction of forwardDirection, for some reason it needs to be negative. The argument of Mathf.Cos is in radians
        body.velocity = new Vector2(moveSpeed * Mathf.Sin(-forwardDirection.z), moveSpeed * Mathf.Cos(-forwardDirection.z));

        // Smoothly rotate the transform to face the forward direction using quaternions, the angle needs to be in degrees for this  
        critterTransform.rotation = Quaternion.Lerp(critterTransform.rotation, rotation, Time.deltaTime * 5f);
    }

    // Basic movement function
    public void Wander()
    {

        currentSpeed = moveSpeed;

        // Get list of tiles around the critter within in some range, weight them according to what biome type there are and chosee one at random as the target
        //int targertTile = Random.Range(0, 9);

        //// Position of target tile
        //Vector2Int targetPosition = new Vector2Int((int)critterTransform.position.x, (int)critterTransform.position.y);

        //int tileType = mapGenerator.GetComponent<BiomeGenerator>().biomeMap[targetPosition.x,targetPosition.y];
        //Debug.Log(tileType);

        // Add velocity to the rigid body inthe direction of forwardDirection, for some reason it needs to be negative. The argument of Mathf.Cos is in radians
        body.velocity = new Vector2(moveSpeed * Mathf.Sin(-forwardDirection.z), moveSpeed * Mathf.Cos(-forwardDirection.z));

        // Smoothly rotate the transform to face the forward direction using quaternions, the angle needs to be in degrees for this  
        critterTransform.rotation = Quaternion.Lerp(critterTransform.rotation, rotation, Time.deltaTime * 5f);
         
    }

    // Stop moving
    public void Idle()
    {
        currentSpeed = 0f;
        body.velocity = new Vector2(0f, 0f);
    }


    public void Persue(GameObject target)
    {
        //
    }

    public void Flee(Transform target)
    {
        currentSpeed = 3f*moveSpeed;
        // Move away from the target 
        // Add velocity to the rigid body inthe direction of forwardDirection, for some reason it needs to be negative. The argument of Mathf.Cos is in radians
        body.velocity = new Vector2(3f*moveSpeed * Mathf.Sin(-forwardDirection.z), 3f*moveSpeed * Mathf.Cos(-forwardDirection.z));

        // Smoothly rotat the transform to face the forward direction using quaternions, the angle needs to be in degrees for this  
        critterTransform.rotation = Quaternion.Lerp(critterTransform.rotation, rotation, Time.deltaTime * 5f);

    }

    public void Herd()
    {
        // Add velocity to the rigid body inthe direction of forwardDirection, for some reason it needs to be negative. The argument of Mathf.Cos is in radians
        body.velocity = new Vector2(moveSpeed * Mathf.Sin(-forwardDirection.z), moveSpeed * Mathf.Cos(-forwardDirection.z));

        // Smoothly rotat the transform to face the forward direction using quaternions, the angle needs to be in degrees for this  
        critterTransform.rotation = Quaternion.Lerp(critterTransform.rotation, rotation, Time.deltaTime * 5f);
    }


}





//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CritterMovement : MonoBehaviour
//{

//    public Transform Player;
//    int MoveSpeed = 4;
//    int MaxDist = 10;
//    int MinDist = 5;




//    void Start()
//    {

//    }

//    void Update()
//    {
//        transform.LookAt(Player, Vector3.forward);

//        if (Vector3.Distance(transform.position, Player.position) >= MinDist)
//        {

//            transform.position += transform.forward * MoveSpeed * Time.deltaTime;



//            if (Vector3.Distance(transform.position, Player.position) <= MaxDist)
//            {
//                //Here Call any function U want Like Shoot at here or something
//            }

//        }
//    }
//}


//using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

//public class CritterMovement : MonoBehaviour
//{


//    //// see https://www.gamedevelopment.blog/full-unity-2d-game-tutorial-2019-simple-enemy-ai-and-colliders/
//    //// movement is in fixed update 

//    //Rigidbody2D body;
//    //Transform playerTransform;
//    //Animator playerAnimator;

//    //float horizontal;
//    //float vertical;
//    //float moveLimiter = 0.7f;
//    //float smooth = 5.0f;

//    //Vector3 forwardDirection = new Vector3(0, 0, 0);

//    //public float runSpeed = 20.0f;
//    ////bool stopIt = True;
//    //bool doIt = false;
//    //int count = 0;

//    // boolean variables to determine wht movement should be performed
//    bool wander = false;
//    bool persue = false;
//    bool flee = false;

//    // Start is called before the first frame update
//    void Start()
//    {
//        // Get a reference to the transform and rigid body attached to this object
//        body = GetComponent<Rigidbody2D>();
//        playerTransform = GetComponent<Transform>();
//        playerAnimator = GetComponent<Animator>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //if (doIt)
//        //{
//        //    // Gives a value between -1 and 1
//        //    vertical = 1.0f;// Random.Range(-1.0f, 1.0f);
//        //    horizontal  = Random.Range(-0.5f, 0.5f);
//        //    count = 0;
//        //    //stopIt = true;
//        //    doIt = false;
//        //    forwardDirection.z -= 2.0f * horizontal;

//        //    //transform.rotation = new Vector3(0.0f, 0.0f, forwardDirection);
//        //    playerTransform.eulerAngles = forwardDirection;
//        //}
//        //if (count == 100)
//        //{
//        //    doIt = true;

//        //}
//        ////if (stopIt)
//        ////{
//        ////    vertical = 0.0f;
//        //}

//    }

//    void FixedUpdate()
//    {

//        if (wander)
//        {
//            Wander();
//        }

//        // Quaternion.Slerp(playerTransform.rotation, forwardDirection, Time.deltaTime * smooth);
//        //Debug.Log(forwardDirection.z);
//        body.velocity = new Vector2(vertical * runSpeed * Mathf.Sin(-Mathf.PI / 180f * forwardDirection.z), vertical * runSpeed * Mathf.Cos(-Mathf.PI / 180f * forwardDirection.z));
//        count += 1;
//    }

//    // Basic movement function
//    public void Wander()
//    {
//        // Chose random direction move in that direction at constant speed for random duration, pause,
//        // repeat

//    }

//    public void Persue(GameObject target)
//    {
//        //
//    }

//}
