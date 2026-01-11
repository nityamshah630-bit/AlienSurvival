using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f; // Adjustable rotation speed
    [SerializeField] private GameObject gameOverCanvas; // Assign GameOverCanvas in Inspector
    [SerializeField] private GameObject successCanvas; // Assign SuccessCanvas in Inspector
    public float fallTimeout = 2f; // Time before game over when not grounded

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private PlayerInputActions inputActions;
    private Animator animator;
    private bool isGameOver = false;
    private float fallTime = 0f; // Tracks time not grounded
    private bool level1Success = false; // Track success state

    void Awake()
    {
        try
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.Enable();
            inputActions.Player.Jump.performed += _ => Jump();
            animator = GetComponent<Animator>();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to initialize inputActions: " + e.Message);
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (gameOverCanvas == null)
        {
            gameOverCanvas = GameObject.Find("GameOverCanvas");
            if (gameOverCanvas == null)
            {
                Debug.LogError("GameOverCanvas not found in the scene! Please assign it in the Inspector or create it.");
            }
            else if (gameOverCanvas.activeSelf)
            {
                gameOverCanvas.SetActive(false);
            }
        }
        if (successCanvas == null)
        {
            successCanvas = GameObject.Find("SuccessCanvas");
            if (successCanvas == null)
            {
                Debug.LogError("SuccessCanvas not found in the scene! Please assign it in the Inspector or create it.");
            }
            else if (successCanvas.activeSelf)
            {
                successCanvas.SetActive(false);
            }
        }
        Debug.Log("Game start: GameOverCanvas active = " + (gameOverCanvas != null ? gameOverCanvas.activeSelf.ToString() : "null") +
                  ", SuccessCanvas active = " + (successCanvas != null ? successCanvas.activeSelf.ToString() : "null"));
    }

    void Update()
    {
        if (isGameOver || level1Success) return; // Stop updates if game over or success

        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            fallTime = 0f; // Reset fall timer when grounded
            if (velocity.y < 0) velocity.y = -2f;
        }
        else
        {
            fallTime += Time.deltaTime;
            Debug.Log("Falling: Fall Time = " + fallTime + ", Grounded = " + isGrounded);
            if (fallTime > fallTimeout)
            {
                GameOver();
            }
        }

        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        float speed = inputActions.Player.Run.IsPressed() ? runSpeed : walkSpeed;
        bool isRunning = inputActions.Player.Run.IsPressed();

        float currentSpeed = moveInput.magnitude * speed;
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * speed;

        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 targetDirection = (cameraRight * moveInput.x + cameraForward * moveInput.y).normalized;

            if (targetDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            move = targetDirection * speed;
        }

        controller.Move(move * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("Speed", currentSpeed);
        Debug.Log($"Current Speed: {currentSpeed}, IsRunning: {isRunning}, IsGrounded: {isGrounded}, Fall Time: {fallTime}");
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("Jump", !isGrounded);
    }

    private void Jump()
    {
        if (isGameOver || level1Success || !isGrounded) return;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator.SetBool("Jump", true);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isGameOver || level1Success) return;

        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("Fence"))
        {
            GameOver();
        }
        else if (hit.gameObject.CompareTag("Collectible"))
        {
            level1Success = true;
            if (successCanvas != null)
            {
                successCanvas.SetActive(true);
                Time.timeScale = 0f; // Pause the game
                Destroy(hit.gameObject); // Remove the item
                Debug.Log("Level 1 success! SuccessCanvas activated.");
            }
            else
            {
                Debug.LogError("SuccessCanvas is null, cannot display message!");
            }
        }
        else if (hit.gameObject.CompareTag("EnemyBoss"))
        {
            GameOver(); // Trigger game over on boss collision
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBoss") && !isGameOver && !level1Success)
        {
            GameOver(); // Trigger game over when entering boss radius
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Pause the game
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            TextMeshProUGUI gameOverText = gameOverCanvas.transform.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();
            if (gameOverText != null)
            {
                gameOverText.text = "Game Over";
            }
            else
            {
                UnityEngine.UI.Text legacyText = gameOverCanvas.transform.Find("GameOverText")?.GetComponent<UnityEngine.UI.Text>();
                if (legacyText != null)
                {
                    legacyText.text = "Game Over";
                }
                else
                {
                    Debug.LogWarning("GameOverText not found or not a TextMeshProUGUI/Text component!");
                }
            }
            Debug.Log("Game Over message displayed! Canvas active: " + gameOverCanvas.activeSelf);
        }
        else
        {
            Debug.LogError("GameOverCanvas is null, cannot display message!");
        }
        Debug.Log("Game Over!");
    }

    public void ProceedToLevel2()
    {
        Debug.Log("Proceed to Level 2 clicked!");
        if (successCanvas != null) successCanvas.SetActive(false);
        Time.timeScale = 1f; // Resume time
        level1Success = false; // Reset success state
        SceneManager.LoadScene("Level2"); // Replace with your Level 2 scene name
        Debug.Log("Level 2 loaded!");
    }

    public void RetryLevel1()
    {
        Debug.Log("Retry Level 1 clicked!");
        if (successCanvas != null) successCanvas.SetActive(false);
        Time.timeScale = 1f; // Resume time
        level1Success = false; // Reset success state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current level
        Debug.Log("Level restarted!");
    }

    public void PlayAgain()
    {
        Debug.Log("Play Again clicked!");
        if (successCanvas != null) successCanvas.SetActive(false); // Disable success canvas
        Time.timeScale = 1f; // Resume time
        level1Success = false; // Reset success state
        SceneManager.LoadScene("Level1"); // Load Level 1 (replace with your Level 1 scene name if different)
        Debug.Log("Level 1 loaded!");
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Disable();
            inputActions.Dispose(); // Clean up to prevent memory leaks
        }
    }
}