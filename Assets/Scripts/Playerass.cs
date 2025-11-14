using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8f;
    public float secondJS = 6f;
    public float gravity = 20f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public Camera playerCamera;
    public Transform camera;
    public Vector3 restPosition;

    public GameObject punchHitbox;
    public float punchRange = 2f;
    public float punchForce = 15f;
    public float punchRecoil = 8f;
    public float punchDelay = 0.5f;
    public float punchCooldown = 0.3f;

    public FlipOffBox flipOffHitbox;

    public float dashSpeed = 50f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;
    public float dashVerticalBoost = 2f;

    public float bobSpeed = 4.8f;
    public float bobAmount = 0.05f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float bobTimer = Mathf.PI / 2;

    private bool canMove = true;
    private bool canDash = true;
    private bool isDashing = false;
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
            StartCoroutine(DoPunch());

        if (Input.GetKey(KeyCode.LeftAlt) && canDash && !characterController.isGrounded)
            StartCoroutine(Dash());
        
            if (Input.GetKeyDown(KeyCode.F))
            {
                flipOffHitbox.Activate(); 
            }
        

    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementY = moveDirection.y;

        moveDirection = forward * curSpeedX + right * curSpeedY;

        if (Input.GetButtonDown("Jump") && canMove && jumpsLeft > 0)
        {
            moveDirection.y = secondJS;
            jumpsLeft--;
        }
        else if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpsLeft = maxJumps;
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementY;
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
            bobTimer += bobSpeed * Time.deltaTime;
            Vector3 newPos = new Vector3(
                Mathf.Cos(bobTimer) * bobAmount,
                restPosition.y + Mathf.Abs(Mathf.Sin(bobTimer) * bobAmount),
                restPosition.z
            );
            camera.localPosition = newPos;
        }
        else
        {
            bobTimer = Mathf.PI / 2;
        }
        if (bobTimer > Mathf.PI * 2) bobTimer = 0;
    }

    private IEnumerator DoPunch()
    {
        canPunch = false;
        punchHitbox.SetActive(true);

        yield return new WaitForSeconds(punchDelay);

        RaycastHit hit;
        Vector3 origin = playerCamera.transform.position;
        Vector3 dir = playerCamera.transform.forward;

        if (Physics.Raycast(origin, dir, out hit, punchRange))
        {
            EnemyKnockback ek = hit.collider.GetComponentInParent<EnemyKnockback>();
            if (ek != null)
            {
                Vector3 knockDir = (hit.point - transform.position);
                knockDir.y = 0f;
                knockDir = knockDir.normalized + Vector3.up * 0.2f;
                ek.Knockback(knockDir, punchForce);
                GameManager.Instance?.AddScore(5);
                GameManager.Instance?.AddTime(1f);
            }
            else
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    rb.AddForce((hit.point - transform.position).normalized * punchForce, ForceMode.Impulse);
                }
                else
                {
                    StartCoroutine(DoRecoil(-dir * punchRecoil));
                }
            }
        }

        yield return new WaitForSeconds(punchCooldown);
        punchHitbox.SetActive(false);
        canPunch = true;
    }

    private IEnumerator DoRecoil(Vector3 recoilDir)
    {
        float duration = 0.1f;
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

        float originalWalk = walkingSpeed;
        float originalRun = runningSpeed;
        float originalFOV = playerCamera.fieldOfView;

        walkingSpeed = dashSpeed;
        runningSpeed = dashSpeed;
        if (!characterController.isGrounded) moveDirection.y = dashVerticalBoost;
        playerCamera.fieldOfView += 10f;

        yield return new WaitForSeconds(dashDuration);

        walkingSpeed = originalWalk;
        runningSpeed = originalRun;
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

    public void ApplyRecoil(Vector3 recoil)
    {
        moveDirection += recoil;
    }
}
