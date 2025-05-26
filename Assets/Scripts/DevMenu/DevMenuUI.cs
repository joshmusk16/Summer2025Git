using UnityEngine;

public class DevMenuUI : MonoBehaviour
{
    [Header("DevMenu Tab Sprites")]
    public Sprite[] tabs = new Sprite[3];
    private int currentTab = 0;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }
 
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            Mathf.Abs(mainCamera.transform.position.z)));

        if (Input.GetKey(KeyCode.Mouse1))
        {
            transform.position = Vector2.Lerp(transform.position, mouseWorldPosition - new Vector2(0, 96) / 16f, Time.deltaTime * 10f);
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = tabs[currentTab];

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (DetectMouse(new Vector2(-67f, 84f), 44f, 11f))
            {
                currentTab = 0;
            }
            else if (DetectMouse(new Vector2(-19f, 84f), 44f, 11f))
            {
                currentTab = 1;
            }
            else if (DetectMouse(new Vector2(28f, 84f), 44f, 11f))
            {
                currentTab = 2;
            }
        }        
    }

    private bool DetectMouse(Vector2 displacement, float xBounds, float yBounds)
    {
        Vector2 startingPosition = (Vector2) transform.position + (displacement / 16f);
        xBounds /= 32f;
        yBounds /= 32f;

        if (mouseWorldPosition.x > startingPosition.x - xBounds
        && mouseWorldPosition.x < startingPosition.x + xBounds
        && mouseWorldPosition.y > startingPosition.y - yBounds
        && mouseWorldPosition.y < startingPosition.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
