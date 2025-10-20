using UnityEngine;

public class DevMenuUI : MonoBehaviour
{
    [Header("DevMenu Tab Sprites")]
    public Sprite[] tabs = new Sprite[3];
    private int currentTab = 0;

    private bool followingMouse = false;

    [Header("TileGrid Reference")]
    public TileGrid tileGrid;

    public MouseTracker mouse;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && DetectMouse(new Vector2(0, 96), 180, 8))
        {
            tileGrid.DisableEditing();
            followingMouse = true;
        }

        if (followingMouse)
        {
            transform.position = Vector2.Lerp(transform.position, mouse.worldPosition - new Vector2(0, 96) / 16f, Time.deltaTime * 10f);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            followingMouse = false;
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

        if (mouse.worldPosition.x > startingPosition.x - xBounds
        && mouse.worldPosition.x < startingPosition.x + xBounds
        && mouse.worldPosition.y > startingPosition.y - yBounds
        && mouse.worldPosition.y < startingPosition.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
