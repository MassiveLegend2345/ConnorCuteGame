using UnityEngine;
using UnityEngine.UI; // ADD THIS LINE

public class CanvasResolutionFix : MonoBehaviour
{
    private void Start()
    {
        FixCanvasScale();
    }

    private void FixCanvasScale()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // This makes UI scale properly on all resolutions
            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f; // Balances between width and height
            }
        }
    }
}