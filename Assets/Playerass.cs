using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(CharacterController))]

public class FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float secondJS = 6.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float boosterspeed = 50.0f;
    public float launchForce = 20.0f;
    public Vector3 restPosition;
    public Transform camera;

    public float bobSpeed = 4.8f;
    public float bobAmount = 0.05f;

    private float timer = Mathf.PI / 2;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    private int maxJumps = 2;
    private int jumpsLeft;


    [Header("Dash Settings")]
    public float dashSpeed = 50f;            // Raw dash power
    public float dashDuration = 0.15f;       // How long dash lasts
    public float dashCooldown = 0.4f;        // Time between dashes
    public float dashVerticalBoost = 2f;     // Small upward lift
    private bool isDashing = false;
    private bool canDash = true;


    void Start()
    {
        characterController = GetComponent<CharacterController>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        jumpsLeft = maxJumps;

    }
    // Locks cursor
    void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<Collider>().tag == "death box")
        {
            characterController.transform.position = new Vector3(0, 26, 0);
            Physics.SyncTransforms();
        }

        if (collision.CompareTag("tele"))    // Changed to use your parameter name
        {
            StartCoroutine(TeleportPlayer(new Vector3(-1f, 26f, 102f)));
        }
    }

    IEnumerator TeleportPlayer(Vector3 newPosition)
    {
        // 1. Disable CharacterController first
        characterController.enabled = false;

        // 2. Set the new position
        transform.position = newPosition;

        // 3. Wait one frame
        yield return null;

        // 4. Re-enable CharacterController
        characterController.enabled = true;

        Debug.Log("Teleported to: " + newPosition);
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //recalculate move direction based on axes

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        // Press Left Shift to run





        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            jumpsLeft = maxJumps;
            moveDirection.y = jumpSpeed;

        }
        else
        {
            moveDirection.y = movementDirectionY;
        }


        if (Input.GetButtonDown("Jump") && canMove && jumpsLeft > 0)
        {
            moveDirection.y = secondJS;
            jumpsLeft -= 1;


        }
        else
        {
            moveDirection.y = movementDirectionY;

        }
        if (Input.GetButton("Jump") && canMove)
        {

        }
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0) && canDash && !characterController.isGrounded)
        {
            StartCoroutine(Dash());
        }
        if (!isDashing) // Only allow normal movement when not dashing
        {
            // Your existing movement calculations here
        }

        // Always apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Always move the character
        characterController.Move(moveDirection * Time.deltaTime);
        // Apply gravity. Gravity is multiplied by deltaTime twice

        characterController.Move(moveDirection * Time.deltaTime);
        // Movement

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        // Player and Camera rotation
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            timer += bobSpeed * Time.deltaTime;

            Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount,
                restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z);
            camera.localPosition = newPosition;
        }
        else
        {
            timer = Mathf.PI / 2;
        }

        if (timer > Mathf.PI * 2)
        {
            timer = 0;
        }

    }



    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Store original values
        float originalWalkSpeed = walkingSpeed;
        float originalRunSpeed = runningSpeed;
        float originalFOV = playerCamera.fieldOfView;

        // Apply dash
        walkingSpeed = dashSpeed;
        runningSpeed = dashSpeed;

        if (!characterController.isGrounded)
        {
            moveDirection.y = dashVerticalBoost;
        }

        // Visual effects (only modify FOV on main camera)
        playerCamera.fieldOfView += 10f;

        yield return new WaitForSeconds(dashDuration);

        // Restore movement
        walkingSpeed = originalWalkSpeed;
        runningSpeed = originalRunSpeed;
        isDashing = false;

        // Smooth FOV restore
        float elapsed = 0f;
        float fadeTime = 0.1f;
        while (elapsed < fadeTime)
        {
            playerCamera.fieldOfView = Mathf.Lerp(originalFOV + 10f, originalFOV, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = originalFOV;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}

