using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float secondJS = 6.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public Camera playerCamera;
    public Transform camera;
    public Vector3 restPosition;

    [Header("Punch Settings")]
    [SerializeField] private GameObject punchHitbox;
    [SerializeField] private float hitboxActiveTime = 0.15f;
    [SerializeField] private float punchRange = 2f;
    [SerializeField] private float punchForce = 15f; // for enemies
    [SerializeField] private float recoilForce = 8f; // for player recoil
    [SerializeField] private float punchCooldown = 0.3f;

    [Header("Head Bob")]
    public float bobSpeed = 4.8f;
    public float bobAmount = 0.05f;

    [Header("Dash Settings")]
    public float dashSpeed = 50f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;
    public float dashVerticalBoost = 2f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float timer = Mathf.PI / 2;
    private bool canMove = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool canPunch = true;

    private int maxJumps = 2;
    private int jumpsLeft;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCameraBob();

        if (Input.GetMouseButtonDown(0) && canPunch)
        {
            StartCoroutine(DoPunch());
        }

        if (Input.GetKey(KeyCode.LeftAlt) && canDash && !characterController.isGrounded)
        {
            StartCoroutine(Dash());
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButtonDown("Jump") && canMove && jumpsLeft > 0)
        {
            moveDirection.y = secondJS;
            jumpsLeft -= 1;
        }
        else if (characterController.isGrounded && Input.GetButton("Jump"))
        {
            jumpsLeft = maxJumps;
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (!canMove) return;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void HandleCameraBob()
    {
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
            timer = 0;
    }

    private IEnumerator DoPunch()
    {
        canPunch = false;
        punchHitbox.SetActive(true);

        // Raycast to detect what we hit
        RaycastHit hit;
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out hit, punchRange))
        {
            // If it has a rigidbody -> knockback enemy/object
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                Vector3 knockDir = (hit.point - transform.position).normalized;
                rb.AddForce(knockDir * punchForce, ForceMode.Impulse);
            }
            else
            {
                // Hit something solid (wall): apply recoil
                Vector3 recoilDir = -direction * recoilForce;
                StartCoroutine(DoRecoil(recoilDir));
            }
        }

        yield return new WaitForSeconds(hitboxActiveTime);
        punchHitbox.SetActive(false);
        yield return new WaitForSeconds(punchCooldown);
        canPunch = true;
    }

    private IEnumerator DoRecoil(Vector3 recoilDir)
    {
        float duration = 0.1f; // quick impulse
        float elapsed = 0f;

        while (elapsed < duration)
        {
            characterController.Move(recoilDir * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalWalkSpeed = walkingSpeed;
        float originalRunSpeed = runningSpeed;
        float originalFOV = playerCamera.fieldOfView;

        walkingSpeed = dashSpeed;
        runningSpeed = dashSpeed;

        if (!characterController.isGrounded)
        {
            moveDirection.y = dashVerticalBoost;
        }

        playerCamera.fieldOfView += 10f;

        yield return new WaitForSeconds(dashDuration);

        walkingSpeed = originalWalkSpeed;
        runningSpeed = originalRunSpeed;
        isDashing = false;

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
