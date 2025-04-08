using System.Collections;
using UnityEngine;

public class SnakeController : Enemy
{
    public float strollSpeed = 1f; // Speed of random strolling
    public float dashSpeed = 5f; // Speed of dashing toward the player
    public float dashInterval = 5f; // Time interval between dashes
    public float dashDuration = 1f; // Duration of the dash
    public float directionChangeInterval = 2f; // Time interval for changing random stroll direction
    public float constrictionScaleFactor = 0.8f; // Minimum X scale during constriction
    public float constrictionSpeed = 5f; // Speed of the constriction animation

    private Vector3 randomDirection; // Current random direction for strolling
    private float lastDirectionChangeTime = 0f; // Tracks the last time the direction was changed
    private float lastDashTime = 0f; // Tracks the last time the snake dashed
    private bool isDashing = false; // Tracks whether the snake is currently dashing
    private Vector3 originalScale; // Original scale of the sprite

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalScale = spriteRenderer.transform.localScale;
        }
        ChangeRandomDirection(); // Initialize the first random direction
    }

    protected override void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;
        if (DialogManager.Instance?.IsRunning() == true) return;

        // Make the enemy visible when the level starts
        UnhideSprite();
        if (spriteRenderer.enabled)
        {
            if (isDashing)
            {
                DashTowardPlayer();
            }
            else
            {
                StrollRandomly();
                CheckForDash();
            }

            RotateTowardsPlayer();
            Attack();
            AnimateConstriction();
        }
    }

    private void StrollRandomly()
    {
        // Change direction at regular intervals
        if (Time.time - lastDirectionChangeTime >= directionChangeInterval)
        {
            ChangeRandomDirection();
        }

        // Move in the current random direction
        rb.velocity = randomDirection * strollSpeed;
    }

    private void ChangeRandomDirection()
    {
        // Generate a random direction on the XZ plane
        randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        lastDirectionChangeTime = Time.time;
    }

    private void CheckForDash()
    {
        // Check if it's time to dash
        if (Time.time - lastDashTime >= dashInterval)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // Calculate the direction toward the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Dash for the specified duration
        float dashEndTime = Time.time + dashDuration;
        while (Time.time < dashEndTime)
        {
            rb.velocity = directionToPlayer * dashSpeed;
            yield return null;
        }

        isDashing = false;
    }

    private void DashTowardPlayer()
    {
        // While dashing, the velocity is already set in the Dash coroutine
        // This method is here for clarity and potential future use
    }

    private void AnimateConstriction()
    {
        if (spriteRenderer == null) return;

        // Calculate the constriction factor based on the current speed
        float speed = rb.velocity.magnitude;
        float targetScaleX = Mathf.Lerp(constrictionScaleFactor, 1f, speed / dashSpeed);

        // Smoothly interpolate the X scale
        Vector3 newScale = spriteRenderer.transform.localScale;
        newScale.x = Mathf.Lerp(newScale.x, originalScale.x * targetScaleX, Time.deltaTime * constrictionSpeed);
        spriteRenderer.transform.localScale = newScale;
    }
}