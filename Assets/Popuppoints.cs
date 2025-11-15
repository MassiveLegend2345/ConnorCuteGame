using System.Collections;
using UnityEngine;
using TMPro;

public class PointsPopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float duration = 1f;

    private Vector3 worldPosition;
    private Camera mainCam;

    public void Setup(string text, Vector3 worldPos, Camera cam)
    {
        if (textMesh != null)
            textMesh.text = text;

        worldPosition = worldPos;
        mainCam = cam;

        StartCoroutine(MoveAndFade());
    }

    private IEnumerator MoveAndFade()
    {
        float elapsed = 0f;
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null)
            cg = gameObject.AddComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move the popup upwards over time
            if (mainCam != null)
                transform.position = mainCam.WorldToScreenPoint(Vector3.Lerp(worldPosition, worldPosition + Vector3.up, t));

            // Fade out
            cg.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}
