
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

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    float smooth = 5.0f;

    Vector3 forwardDirection = new Vector3(0, 0, 0);
    //Quaternion forwardDirection = Quaternion.Euler(0, 0, 0);

    public float runSpeed = 20.0f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

   
    }

    void FixedUpdate()
    {
        //if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        //{
        //    // limit movement speed diagonally, so you move at 70% speed
        //    horizontal *= moveLimiter;
        //    vertical *= moveLimiter;
        //}
        forwardDirection.z -= 2.0f*horizontal;

        //transform.rotation = new Vector3(0.0f, 0.0f, forwardDirection);
        playerTransform.eulerAngles = forwardDirection; // Quaternion.Slerp(playerTransform.rotation, forwardDirection, Time.deltaTime * smooth);
        //Debug.Log(forwardDirection.z);
        body.velocity = new Vector2(vertical * runSpeed*Mathf.Sin(-Mathf.PI/180f*forwardDirection.z), vertical * runSpeed*Mathf.Cos(-Mathf.PI / 180f * forwardDirection.z));


        //playerAnimator.SetFloat("Speed", vertical * runSpeed);

    }

}