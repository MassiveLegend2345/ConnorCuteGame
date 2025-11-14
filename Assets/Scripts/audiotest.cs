/*using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip testClip;
    public float volume = 1f;
    public float pitch = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[AudioTest] T pressed -> testing audio playback");
            if (audioSource == null)
            {
                Debug.LogError("[AudioTest] audioSource is null! Assign an AudioSource in inspector.");
                return;
            }
            if (testClip == null)
            {
                Debug.LogError("[AudioTest] testClip is null! Assign an AudioClip in inspector.");
                return;
            }

            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(testClip);
            Debug.Log("[AudioTest] PlayOneShot called.");
        }
    }
}
*/