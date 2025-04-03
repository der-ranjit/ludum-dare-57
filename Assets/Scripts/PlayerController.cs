using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody rigidBody;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        // log
        Debug.Log($"MoveX: {moveX}, MoveZ: {moveZ}");
        // log rigidbody velocity
        // stop instantly if no input
        if (moveX == 0 && moveZ == 0)
        {
            rigidBody.velocity = new Vector3(0f, rigidBody.velocity.y, 0f);
            return;
        }

        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        // preserve y velocity
        rigidBody.velocity = moveDirection * moveSpeed + new Vector3(0f, rigidBody.velocity.y, 0f);
    }
}