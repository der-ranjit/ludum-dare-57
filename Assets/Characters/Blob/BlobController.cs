using UnityEngine;

public class BlobController : Enemy
{

    public float jumpInterval = 2f; // Time interval between jumps
    public float jumpForce = 5f; // Force applied when jumping
    private float lastJumpTime = 0f; // Tracks the last time the enemy jumped
    private IsGroundedCheck isGroundedCheck;

    protected override void Start()
    {
        base.Start();
        isGroundedCheck = GetComponentInChildren<IsGroundedCheck>(); // Get the IsGroundedCheck component
        jumpInterval += Random.Range(0.8f, 1.2f);
        jumpForce *= Random.Range(0.9f, 1.5f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;
        if (DialogManager.Instance?.IsRunning() == true) return;
        Attack();
        JumpPeriodically();
    }

    private void JumpPeriodically()
    {
        // Add jump force if grounded and enough time has passed
        if (Time.time - lastJumpTime >= jumpInterval && isGroundedCheck != null && isGroundedCheck.IsGrounded())
        {
            Debug.Log("Blob is jumping!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            lastJumpTime = Time.time; // Update the last jump time
        }
    }
}
