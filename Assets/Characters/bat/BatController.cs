using System.Collections;
using UnityEngine;

public class BatController : Enemy
{

    public float lockedHeight = 1f; // The height at which the bat should stay
    public float shittingTime = 1f;
    public float shittingInterval = 6f; // Time interval between jumps
    private float lastShittingTime = 0f; // Tracks the last time the enemy jumped
    private bool isShitting = false; // Tracks whether the bat is currently shitting

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;
        if (DialogManager.Instance?.IsRunning() == true) return;
        Unhide();
        RotateTowardsPlayer();
        ShitPeriodically();
    }

    private void ShitPeriodically()
    {
        if (playerTransform == null)
        {
            return;
        }
        if (isShitting)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            // Calculate the target position for the next frame
            Vector3 targetPosition = transform.position + directionToPlayer * moveSpeed * Time.fixedDeltaTime;
            targetPosition.y = lockedHeight;
            transform.position = targetPosition;
        }
        // Check if enough time has passed since the last shitting action
        if (Time.time - lastShittingTime >= shittingInterval && !isShitting)
        {
            StartCoroutine(PerformShitting());
        }
    }


    private IEnumerator PerformShitting()
    {
        isShitting = true; // Set the shitting state to true
        Debug.Log("Bat is shitting!");

        // Stop moving while shitting
        Vector3 originalVelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        // Wait for the shitting duration
        yield return new WaitForSeconds(shittingTime);
        Attack();

        // Resume normal behavior
        rb.velocity = originalVelocity;
        lastShittingTime = Time.time; // Update the last shitting time
        isShitting = false; // Reset the shitting state
        Debug.Log("Bat finished shitting!");
    }
}
