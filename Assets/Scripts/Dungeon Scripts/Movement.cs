using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Rigidbody2D rb;
    private Vector2 movement;

    //updates movement vector based on player input
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    //moves the player and rotates towards movement direction
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement != Vector2.zero)
        {
            float targetAngle = 0f;

            if (movement.y > 0)
            {
                targetAngle = 180f;
            }
            else if (movement.y < 0)
            {
                targetAngle = 0f;
            }
            else if (movement.x > 0)
            {
                targetAngle = 90f;
            }
            else if (movement.x < 0)
            {
                targetAngle = -90f;
            }

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    //handles interaction with Button objects on trigger enter
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Button"))
        {
            ButtonScript buttonScript = other.GetComponent<ButtonScript>();
            if (buttonScript != null)
            {
                buttonScript.PressButton();
            }
        }
    }
}
