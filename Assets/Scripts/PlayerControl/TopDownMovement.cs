
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TopDownMovement : MonoBehaviour
{

    Rigidbody2D body;
    Transform playerTransform;
    Animator playerAnimator;

    Transform parentTransform;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    float smooth = 5.0f;
    Vector2 mousePos;

    Vector3 forwardDirection = new Vector3(0, 0, 0);
    //Quaternion forwardDirection = Quaternion.Euler(0, 0, 0);

    public float runSpeed;

    // Reference to the arm
    public Transform playerArm;



    float forwardAngle;

    public GameObject text;
    public GameObject text2;
    public GameObject text3;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        parentTransform = playerTransform.parent;

        text.GetComponent<Text>().text = "Text";
        //text.Text = "Text";
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        // Get position of cursor
        mousePos = Input.mousePosition;

        //v3.z = 10.0;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        //text.GetComponent<Text>().text = mousePos.ToString();
        //text2.GetComponent<Text>().text = playerTransform.position.ToString();

    }

    void FixedUpdate()
    {






        //if (Input.GetKeyDown("space"))
        //{
        //    Debug.Log(mousePos);
        //}
            //if (horizontal != 0 && vertical != 0) // Check for diagonal movement
            //{
            //    // limit movement speed diagonally, so you move at 70% speed
            //    horizontal *= moveLimiter;
            //    vertical *= moveLimiter;
            //}
           
        forwardDirection.z -= 2.0f*horizontal;




        forwardDirection.z = forwardDirection.z % 360; // - 180f;



        playerTransform.eulerAngles = forwardDirection; // Quaternion.Slerp(playerTransform.rotation, forwardDirection, Time.deltaTime * smooth);
        body.velocity = new Vector2(vertical * runSpeed * Mathf.Sin(-Mathf.PI / 180f * forwardDirection.z), vertical * runSpeed * Mathf.Cos(-Mathf.PI / 180f * forwardDirection.z));



        ////am.WorldToScreenPoint(target.position);


        ////playerAnimator.SetFloat("Speed", vertical * runSpeed);

        //Vector2 dir = mousePos - (Vector2)playerTransform.position;

        //Vector3 playerWorldPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        //Vector2 mouseWorldPos = Input.mousePosition;

        //text3.GetComponent<Text>().text = (mouseWorldPos).ToString();
        //text2.GetComponent<Text>().text = (playerWorldPos).ToString();

        //Vector2 forwardDir = (Vector2)playerWorldPos - mouseWorldPos;
        //float newPoo = 180f - 180f / Mathf.PI * Mathf.Atan2(forwardDir.x, forwardDir.y);

        //forwardDirection.z = newPoo;

        //// Find the angle between the player and the cursor
        //forwardAngle = Mathf.Atan2(playerTransform.position.x - mousePos.x, playerTransform.position.y - mousePos.y);
        //Vector3 poo = new Vector3(0f, 0f, 180f - 180f/Mathf.PI*forwardAngle);

        ////playerArm.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2());
        ////playerArm.right = dir;
        //playerArm.eulerAngles = poo;
        //// Change hegiht of camera when entering building

        ////text3.GetComponent<Text>().text = (-forwardDirection.z).ToString();
        ////text2.GetComponent<Text>().text = (-poo.z).ToString();
        //text.GetComponent<Text>().text = (forwardDirection.z).ToString();


        //playerTransform.eulerAngles = poo;
        ////body.velocity = new Vector2(vertical * runSpeed * Mathf.Sin(-Mathf.PI / 180f * poo.z), vertical * runSpeed * Mathf.Cos(-Mathf.PI / 180f * poo.z));

        ////transform.rotation = new Vector3(0.0f, 0.0f, forwardDirection);
        ////playerTransform.eulerAngles = forwardDirection; // Quaternion.Slerp(playerTransform.rotation, forwardDirection, Time.deltaTime * smooth);
        ////Debug.Log(forwardDirection.z);
        //body.velocity = new Vector2(vertical * runSpeed * Mathf.Sin(-Mathf.PI / 180f * forwardDirection.z), vertical * runSpeed * Mathf.Cos(-Mathf.PI / 180f * forwardDirection.z));

        ////if (forwardDirection.z - poo.z )


        //Vector3 aaa = new Vector3(forwardDir.x, forwardDir.y, 0f);

        ////playerTransform.LookAt(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f));
        //playerTransform.right = aaa;// playerWorldPos - (Vector3)mouseWorldPos;
    }

}