using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public PlayerStats playerStats;
    private Weapon currentWeapon;
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
    private float lastTextTime = 0;
    public WeaponStats powerUpStats;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        targetRotationY = transform.eulerAngles.y; // Initialize target rotation
        cameraTransform = Camera.main.transform; // Get the main camera's transform
        currentWeapon = GetComponentInChildren<Weapon>(); // Get the weapon component
        powerUpStats = ScriptableObject.CreateInstance<WeaponStats>(); // Initialize power-up stats
        // make sure player has a weapon holder transform
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing)
        {
            return;
        }

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

        // Fire weapon
        if (Input.GetButton("Fire1"))
        {
            if (currentWeapon != null)
            {
                Vector3 forward = transform.Find("WeaponHolder")?.forward ?? transform.forward;
                currentWeapon.Attack(forward);
            }
        }

        // Smoothly rotate player in 90-degree increments
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            targetRotationY -= 90f; // Rotate counterclockwise
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            targetRotationY += 90f; // Rotate clockwise
        }

        // Smoothly interpolate to the target rotation
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);
    }


    public void TakeDamage(float damage)
    {
        Debug.Log($"Player took {damage} damage!");
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // Handle player death (e.g., restart level, show game over screen)
    }

    public void CollectPowerUp()
    {
        // Update the weapon's adjusted stats when a power-up is collected
        if (currentWeapon != null)
        {
            currentWeapon.ApplyPlayerPowerUpStats(powerUpStats);
        }
    }


    public void EquipWeapon(Weapon newWeapon)
    {
        // Equip a new weapon and update its adjusted stats
        currentWeapon = newWeapon;
        currentWeapon.ApplyPlayerPowerUpStats(powerUpStats);
    }

    string[] jumpTexts = new string[] { "Oof", "Haa", "Whee", "Wohoo" };

    void Jump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce, rigidBody.velocity.z);

        if (Time.time - lastTextTime < 0.3f)
        {
            return; // Prevent spamming the text effect
        }
        lastTextTime = Time.time;
        string randomString = jumpTexts[UnityEngine.Random.Range(0, jumpTexts.Length)];
        if (UnityEngine.Random.Range(0, 200) < 1)
        {
            randomString = "We have no sound effects!";
        }
        TextParticleSystem.ShowEffect(transform.position + Vector3.up * 0.1f, randomString);
    }

    string[] jumpFromEnemyTexts = new string[] { "Pow!", "Blam!", "Bam!", "Wham!", "Kapow!", "Splash!", "Boom!", "Ouch!" };

    public void JumpFromEnemy()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, jumpForce * 1.3f, rigidBody.velocity.z);

        if (Time.time - lastTextTime < 0.3f)
        {
            return; // Prevent spamming the text effect
        }
        lastTextTime = Time.time;
        string randomString = jumpFromEnemyTexts[UnityEngine.Random.Range(0, jumpFromEnemyTexts.Length)];
        TextParticleSystem.ShowEffect(transform.position + Vector3.up * 0.1f, randomString);
    }

    // Called by player ground collider
    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
    }

    // Method to apply a power-up to the player's stats
    public void ApplyPowerUp(WeaponStats powerUp)
    {
        powerUpStats.damage *= powerUp.damage;
        powerUpStats.bulletSpeed *= powerUp.bulletSpeed;
        powerUpStats.bulletLifetime *= powerUp.bulletLifetime;
        powerUpStats.bulletRange *= powerUp.bulletRange;
        powerUpStats.pushForce *= powerUp.pushForce;
        powerUpStats.fireRate *= powerUp.fireRate;
        powerUpStats.size *= powerUp.size;
        powerUpStats.piercing += powerUp.piercing; // Additive for piercing
        powerUpStats.bulletBounce += powerUp.bulletBounce; // Additive for bounce

        currentWeapon.ApplyPlayerPowerUpStats(powerUpStats); // Update the weapon's stats        
    }

    // draw player forward and weapon holder forward gizmos
    private void OnDrawGizmos()
    {
        Transform weaponHolder = transform.Find("WeaponHolder");
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        if (currentWeapon != null && weaponHolder != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(weaponHolder.position, weaponHolder.position + weaponHolder.forward * 2f);
        }
    }
}