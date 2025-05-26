using UnityEngine;

public class Button : MonoBehaviour
{

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;
    private bool isPressed = false;

    public int lowestSortingOrder;
    public SingleLineText text;
    public SpriteRenderer buttonBottom;

    public bool activated = false; //This bool must be reset to false in another script

    void Start()
    {
        mainCamera = Camera.main;
        buttonBottom.sortingOrder = lowestSortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = lowestSortingOrder + 1;
        text.sortingOrder = lowestSortingOrder + 2;
    }
    
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            Mathf.Abs(mainCamera.transform.position.z)));

        if (Input.GetKeyDown(KeyCode.Mouse0) && DetectMouse(gameObject, 88, 14) && !isPressed)
        {
            gameObject.transform.position += new Vector3(-0.125f, -0.125f, 0);
            isPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) && isPressed)
        {
            gameObject.transform.position -= new Vector3(-0.125f, -0.125f, 0);
            isPressed = false;
            activated = true;
        }
    }
    
private bool DetectMouse(GameObject obj, float xBounds, float yBounds)
    {
        xBounds /= 32f;
        yBounds /= 32f;

        if (mouseWorldPosition.x > obj.transform.position.x - xBounds
        && mouseWorldPosition.x < obj.transform.position.x + xBounds
        && mouseWorldPosition.y > obj.transform.position.y - yBounds
        && mouseWorldPosition.y < obj.transform.position.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
