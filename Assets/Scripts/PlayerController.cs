using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float rotationSpeed = 5f; // Speed of smooth rotation
    private bool isGrounded = true;
    private bool canDoubleJump = false;
    private float lastMoveX = 0f;
    private float targetRotationY = 0f; // Target Y rotation for smooth rotation
    private Rigidbody rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform cameraTransform; // Reference to the camera's transform


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        targetRotationY = transform.eulerAngles.y; // Initialize target rotation
        cameraTransform = Camera.main.transform; // Get the main camera's transform
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        // Get the camera's forward and right vectors
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.Normalize();
        cameraRight.Normalize();
        // Calculate the movement direction relative to the camera
        Vector3 moveDirection = (cameraForward * moveZ + cameraRight * moveX).normalized;
        rigidBody.velocity = new Vector3(moveDirection.x * moveSpeed, rigidBody.velocity.y, moveDirection.z * moveSpeed);

        // Set animator parameters
        bool isWalking = moveX != 0 || moveZ != 0;
        animator.SetBool("isWalking", isWalking);

        // Flip sprite based on direction
        if (moveX != 0)
        {
            lastMoveX = moveX;
            spriteRenderer.flipX = lastMoveX > 0;
        }

        // Jumping
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

        // Smoothly rotate player in 90-degree increments
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetRotationY -= 90f; // Rotate counterclockwise
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            targetRotationY += 90f; // Rotate clockwise
        }

        // Smoothly interpolate to the target rotation
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);
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