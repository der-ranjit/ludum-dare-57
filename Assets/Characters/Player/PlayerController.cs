using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    public PlayerStats playerStats;
    public WeaponStats rangedWeaponStats;
    public WeaponStats meleeWeaponPrefabStats;
    private Weapon currentWeapon;
    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float rotationSpeed = 5f; // Speed of smooth rotation
    private bool canDoubleJump = false;
    private float lastMoveX = 0f;
    private float targetRotationY = 0f; // Target Y rotation for smooth rotation
    private Rigidbody rigidBody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform cameraTransform; // Reference to the camera's transform
    private float lastTextTime = 0;
    public WeaponStats powerUpStats;
    private IsGroundedCheck isGroundedCheck; // Reference to the IsGroundedCheck component

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        targetRotationY = transform.eulerAngles.y; // Initialize target rotation
        cameraTransform = Camera.main.transform; // Get the main camera's transform
        currentWeapon = GetComponentInChildren<Weapon>(); // Get the weapon component
        isGroundedCheck = GetComponentInChildren<IsGroundedCheck>(); // Get the IsGroundedCheck component
        powerUpStats = ScriptableObject.CreateInstance<WeaponStats>(); // Initialize power-up stats
        // make sure player has a weapon holder transform
        setHp(playerStats.currentHealth); // Set initial health
    }

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing)
        {
            return;
        }

        // Smoothly interpolate to the target rotation
        float currentY = transform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, targetRotationY, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, newY, 0f);

        if (DialogManager.Instance?.IsRunning() == true)
        {
            return; // Prevent player movement during dialog
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
            spriteRenderer.flipX = lastMoveX <= 0;
        }

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGroundedCheck.IsGrounded())
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

        // Change Weapon 
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            if (currentWeapon != null)
            {
                if (currentWeapon.baseStats.weaponType == WeaponStats.WeaponType.Melee)
                {
                    currentWeapon.baseStats = rangedWeaponStats;
                }
                else
                {
                    currentWeapon.baseStats = meleeWeaponPrefabStats;
                }
                currentWeapon.ApplyPlayerPowerUpStats(powerUpStats);
                currentWeapon.AttachToCharacter();
            }
        }
    }


    public void TakeDamage(float damage)
    {
        Debug.Log($"Player took {damage} damage!");
        setHp(playerStats.currentHealth - damage);

        StartCoroutine(FlashRed()); // Flash red on damage

        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    private void setHp(float hp)
    {
        playerStats.currentHealth = hp;
        HealthUI[] healthUIs = FindObjectsOfType<HealthUI>();
        foreach (HealthUI healthUI in healthUIs)
        {
            int intHp = Mathf.CeilToInt(hp / 5);
            healthUI.SetHitpoints(intHp);
        }
    }

    public void Die()
    {
        Debug.Log("Player died!");
        setHp(0);
        GameManager.Instance.ReplayRoomIn(2);
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

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        // Get the material and save the original color
        Material material = spriteRenderer.material;

        // Set the color to red
        material.SetColor("_TintColor", Color.red);
        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);
        // Revert to the original color
        material.SetColor("_TintColor", Color.white);
    }

    public void SetTargetRotation(float targetRotationY)
    {
        this.targetRotationY = targetRotationY;
    }
}