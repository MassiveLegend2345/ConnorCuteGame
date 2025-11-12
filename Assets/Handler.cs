using UnityEngine;
using UnityEngine.UI;

public class UIClickHandler : MonoBehaviour
{
    [Header("UI Elements")]
    public RawImage idleImage;            // Still image

    [Header("Mouse0 Animation")]
    public Animator mouse0Animator;       // Animator for Mouse0 tap
    public float mouse0Duration = 0.5f;   // Duration of Mouse0 animation in seconds

    [Header("F Animation")]
    public Animator fAnimator;            // Animator for F tap
    public float fDuration = 1f;          // Duration of F animation in seconds

    private bool isPlaying = false;       // Is any animation playing

    private void Start()
    {
        // Show only idle image at start
        idleImage.gameObject.SetActive(true);

        // Hide animators at start
        if (mouse0Animator != null) mouse0Animator.gameObject.SetActive(false);
        if (fAnimator != null) fAnimator.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPlaying) return; // Ignore input while animation is playing

        // Mouse0 tap
        if (Input.GetMouseButtonDown(0) && mouse0Animator != null)
        {
            PlayAnimation(mouse0Animator, mouse0Duration);
        }

        // F tap
        if (Input.GetKeyDown(KeyCode.F) && fAnimator != null)
        {
            PlayAnimation(fAnimator, fDuration);
        }
    }

    private void PlayAnimation(Animator animator, float duration)
    {
        isPlaying = true;

        // Hide idle
        idleImage.gameObject.SetActive(false);

        // Show and play animator
        animator.gameObject.SetActive(true);
        animator.Play(0); // play default animation from start

        // Schedule return to idle
        Invoke(nameof(EndAnimation), duration);
    }

    private void EndAnimation()
    {
        // Hide both animators
        if (mouse0Animator != null) mouse0Animator.gameObject.SetActive(false);
        if (fAnimator != null) fAnimator.gameObject.SetActive(false);

        // Show idle
        idleImage.gameObject.SetActive(true);
        isPlaying = false;
    }
}
