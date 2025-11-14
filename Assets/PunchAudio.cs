using UnityEngine;

public class PunchAudioManager : MonoBehaviour
{
    public static PunchAudioManager Instance { get; private set; }

    [Header("Punch Clips (will cycle)")]
    public AudioClip[] punchClips;
    public float volume = 1f;
    public float spatialBlend = 0f; // 0 = 2D, 1 = 3D

    private int currentIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // optional: DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Play the next punch clip at 'position'. Safe to call from any script.
    /// </summary>
    public void PlayNext(Vector3 position)
    {
        if (punchClips == null || punchClips.Length == 0) return;

        AudioClip clip = punchClips[currentIndex];
        if (clip == null) return;

        // Create a one-shot AudioSource GameObject so we can set spatialBlend if needed.
        GameObject go = new GameObject("PunchSFX");
        go.transform.position = position;
        AudioSource src = go.AddComponent<AudioSource>();
        src.spatialBlend = spatialBlend;
        src.volume = volume;
        src.PlayOneShot(clip);
        Destroy(go, clip.length + 0.1f);

        currentIndex = (currentIndex + 1) % punchClips.Length;
    }

    /// <summary>
    /// Convenience: play at manager's position if you don't care about position.
    /// </summary>
    public void PlayNext()
    {
        PlayNext(transform.position);
    }
}
