using UnityEngine;

public class EnemyAudioOnHit : MonoBehaviour
{
    [Header("Punch Hit Sounds")]
    public AudioSource punchAudioSource; // attach on enemy
    public AudioClip[] punchHitClips;    // assign 2 clips here
    private int currentClipIndex = 0;

    public void PlayPunchSound()
    {
        if (punchHitClips.Length == 0 || punchAudioSource == null) return;

        punchAudioSource.PlayOneShot(punchHitClips[currentClipIndex]);
        currentClipIndex = (currentClipIndex + 1) % punchHitClips.Length;
    }
}
