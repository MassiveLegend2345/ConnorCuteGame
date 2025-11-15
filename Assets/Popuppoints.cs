using UnityEngine;
using TMPro;

public class PointsPopup : MonoBehaviour
{
    public float floatSpeed = 50f;  // pixels per second
    public float lifetime = 1f;

    private TextMeshProUGUI tmp;
    private RectTransform rect;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Sets the text and positions it on screen over the world position
    /// </summary>
    public void Setup(string text, Vector3 worldPosition, Camera cam)
    {
        if (tmp != null)
            tmp.text = text;

        if (cam != null && rect != null)
            rect.position = cam.WorldToScreenPoint(worldPosition);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (rect != null)
            rect.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (tmp != null)
        {
            Color c = tmp.color;
            c.a -= Time.deltaTime / lifetime;
            tmp.color = c;
        }
    }
}
