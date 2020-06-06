using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    float moveSpeed;

    float vertical;
    float horizontal;

    Rigidbody2D body;
    Transform playerTransform;
    public Transform playerArm;

    Vector2 mousePos;

    // Vector and quaternion for storing the direction the sprite is currently facing
    Vector3 forwardDirection = new Vector3(0, 0, 0);
    Quaternion rotation;
    float forwardAngle;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        // This corresponds to both WASD and the arrow keys, each input gives a float between -1 and 1
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        // The shift key changes the run speed
        if (Input.GetKey("left shift"))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        // Get position of cursor in screen pixels and convert it to world coordinates
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
    }


    void FixedUpdate()
    {
        // Make the camera follow the player without rotating
        Camera.main.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, -20f);

        // Compute the angle between the player and the cursor position
        forwardAngle = Mathf.Atan2(playerTransform.position.x - mousePos.x, playerTransform.position.y - mousePos.y);

        // Vector to mouse point
        forwardDirection.z = 180f - 180f / Mathf.PI * forwardAngle; // Random.Range(-Mathf.PI, Mathf.PI);

        // This is converted to a quaternion for use in the Lerp function 
        rotation = Quaternion.Euler(forwardDirection);

        // If the player is stationary just the arm moves when the angle is small, otherwise the whole body rotates
        if (body.velocity.magnitude < 0.1f && Mathf.Abs(playerTransform.eulerAngles.z - forwardDirection.z) < 40)
        {
            // Smoothly rotate the transform to face the forward direction using quaternions, the angle needs to be in degrees for this  
            playerArm.rotation = Quaternion.Lerp(playerArm.rotation, rotation, Time.deltaTime * 5f);
        }
        else
        {
            // Both the body and the arm are rotated, this may not be neccessary
            playerArm.rotation = Quaternion.Lerp(playerArm.rotation, rotation, Time.deltaTime * 5f);
            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, rotation, Time.deltaTime * 5f);
        }

        // Forward/backward and side to side movement are not allowed to happen at the same time. It mihg tbe bteer to allow
        // this and set the speed appropriately
        if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal))
        {
            body.velocity = new Vector2(-vertical * moveSpeed * Mathf.Sin(forwardAngle), -vertical * moveSpeed * Mathf.Cos(forwardAngle));
        }
        else
        {
            body.velocity = new Vector2(-horizontal * moveSpeed / 2f * Mathf.Cos(forwardAngle), -horizontal * moveSpeed / 2f * Mathf.Sin(forwardAngle));
        }
    }
}
