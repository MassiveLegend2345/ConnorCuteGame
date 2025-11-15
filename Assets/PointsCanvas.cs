using UnityEngine;

public class PointsCanvas : MonoBehaviour
{
    public static PointsCanvas instance;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
