using UnityEngine;
using UnityEngine.UI;

public class UIClickHandler : MonoBehaviour
{
    public RawImage idleImage;            
    public Animator mouse0Animator;     
    public float mouse0Duration = 0.5f; 
    public Animator fAnimator;           
    public float fDuration = 1f;          
    public AudioSource audioSource;       
    public AudioClip[] fClips;            
    private bool isPlaying = false;       
    private int currentClipIndex = 0;     

    private void Start()
    {
        idleImage.gameObject.SetActive(true);
        if (mouse0Animator != null) mouse0Animator.gameObject.SetActive(false);
        if (fAnimator != null) fAnimator.gameObject.SetActive(false);
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (isPlaying) return; 

        if (Input.GetMouseButtonDown(0) && mouse0Animator != null)
        {
            PlayAnimation(mouse0Animator, mouse0Duration);
        }

        if (Input.GetKeyDown(KeyCode.F) && fAnimator != null)
        {
            PlayAnimation(fAnimator, fDuration);
            PlayNextClip();
        }
    }

    private void PlayAnimation(Animator animator, float duration)
    {
        isPlaying = true;
        idleImage.gameObject.SetActive(false);
        animator.gameObject.SetActive(true);
        animator.Play(0); 
        Invoke(nameof(EndAnimation), duration);
    }

    private void EndAnimation()
    {
        if (mouse0Animator != null) mouse0Animator.gameObject.SetActive(false);
        if (fAnimator != null) fAnimator.gameObject.SetActive(false);

        idleImage.gameObject.SetActive(true);
        isPlaying = false;
    }

    private void PlayNextClip()
    {
        if (fClips.Length == 0) return;
        audioSource.clip = fClips[currentClipIndex];
        audioSource.Play();
        currentClipIndex = (currentClipIndex + 1) % fClips.Length;
    }
}
