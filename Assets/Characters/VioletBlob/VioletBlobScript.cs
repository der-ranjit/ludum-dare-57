using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletBlobScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Jumping true if y velocity is greater than 0.1f
        bool isJumping = GetComponent<Rigidbody>().velocity.y > 0.1f;
        // GetComponentInChildren<Animator>().SetBool("isJumping", isJumping);

        // TODO: death
        // bool isDead = false;
        // GetComponentInChildren<Animator>().SetBool("hasDied", isDead);
    }
}
