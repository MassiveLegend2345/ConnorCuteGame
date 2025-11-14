using UnityEngine;

public class EnemyAudioOnHit : MonoBehaviour
{
    public AudioSource punchAudioSource; 
    public AudioClip[] punchHitClips;    
    private int currentClipIndex = 0;

    public void PlayPunchSound()
    {
        if (punchHitClips.Length == 0 || punchAudioSource == null) return;

        punchAudioSource.PlayOneShot(punchHitClips[currentClipIndex]);
        currentClipIndex = (currentClipIndex + 1) % punchHitClips.Length;
    }
}
