using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private bool canDoubleJump = false;
    private float lastMoveX = 0f; 
    private Rigidbody rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        rigidBody.velocity = new Vector3(moveDirection.x * moveSpeed, rigidBody.velocity.y, moveDirection.z * moveSpeed);

        // set animator parameters
        bool isWalking = moveX != 0 || moveZ != 0;
        animator.SetBool("isWalking", isWalking);

        // flip sprite based on direction
        if (moveX != 0)
        {
            lastMoveX = moveX;
            // TODO fix tessa sprite direction
            spriteRenderer.flipX = lastMoveX > 0;
        }

        // jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }
    }

    void Jump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);
    }

    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
    }
}