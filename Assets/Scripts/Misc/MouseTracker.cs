using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    public Vector2 worldPosition;
    public Vector2 uiPosition;

    private Camera mainCamera;
    private Camera uiCamera;

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
    }

    public Vector3 GetWorldMousePosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)));

        return worldPosition;
    }

    public Vector3 GetUIMousePosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        uiPosition = uiCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(uiCamera.transform.position.z)));

        return uiPosition;
    }

}
