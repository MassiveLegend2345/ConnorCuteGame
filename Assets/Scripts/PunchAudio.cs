using UnityEngine;

public class PunchAudioManager : MonoBehaviour
{
    public static PunchAudioManager Instance { get; private set; }


    public AudioClip[] punchClips;
    public float volume = 1f;
    public float spatialBlend = 0f; 

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayNext(Vector3 position)
    {
        if (punchClips == null || punchClips.Length == 0) return;

        AudioClip clip = punchClips[currentIndex];
        if (clip == null) return;
        GameObject go = new GameObject("PunchSFX");
        go.transform.position = position;
        AudioSource src = go.AddComponent<AudioSource>();
        src.spatialBlend = spatialBlend;
        src.volume = volume;
        src.PlayOneShot(clip);
        Destroy(go, clip.length + 0.1f);

        currentIndex = (currentIndex + 1) % punchClips.Length;
    }
    public void PlayNext()
    {
        PlayNext(transform.position);
    }
}
