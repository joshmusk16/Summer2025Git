using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    [Header("Tile Sprites")]
    public Sprite[] tiles = new Sprite[3];
    private int state = 1;

    [Header("Tile Dimensions")]
    public int pixelWidth;
    public int pixelHeight;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    public bool objectOnTile = true;
    public bool editable = true;

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

        gameObject.GetComponent<SpriteRenderer>().sprite = tiles[state];

        if (editable)
        {
            if (MouseDetected() && state == 1)
            {
                state = 2;
            }
            else if (MouseDetected() == false && state != 0)
            {
                state = 1;
            }

            if (MouseDetected())
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    state = 0;
                    objectOnTile = false;
                }
                else if (Input.GetKey(KeyCode.Mouse1))
                {
                    state = 2;
                    objectOnTile = true;
                }
            }
        }
    }

    public bool MouseDetected()
    {
        float xBounds = pixelWidth / 32f;
        float yBounds = pixelHeight / 32f;

        if (mouseWorldPosition.x > transform.position.x - xBounds
        && mouseWorldPosition.x < transform.position.x + xBounds
        && mouseWorldPosition.y > transform.position.y - yBounds
        && mouseWorldPosition.y < transform.position.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SwitchObjectOnTile()
    {
        if (objectOnTile)
        {
            objectOnTile = false;
        }
        else
        {
            objectOnTile = true;
        }
    }
}
