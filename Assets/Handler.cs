using UnityEngine;
using UnityEngine.UI;

public class UIClickHandler : MonoBehaviour
{
    public RawImage rawImageElement;
    public Image imageElement;

    private void Start()
    {
        // Ensure initial state
        if (rawImageElement != null) rawImageElement.gameObject.SetActive(true);
        if (imageElement != null) imageElement.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleUIElements();
        }
        if (Input.GetMouseButtonUp(0))
        {
            UnToggleUIElements();
        }
    }

    private void ToggleUIElements()
    {
        if (rawImageElement != null)
            rawImageElement.gameObject.SetActive(!rawImageElement.gameObject.activeSelf);

        if (imageElement != null)
            imageElement.gameObject.SetActive(!imageElement.gameObject.activeSelf);
    }
    private void UnToggleUIElements()
    {

        if (imageElement != null)
            imageElement.gameObject.SetActive(!imageElement.gameObject.activeSelf);

        if (rawImageElement != null)
            rawImageElement.gameObject.SetActive(!rawImageElement.gameObject.activeSelf);


    }
}