using UnityEngine;

public class PlaceableItemUI : MonoBehaviour
{
    [Header("Prefab Object and Sprites")]
    public Sprite pickUpSprite;
    public GameObject prefabToPlace;
    public int sortingOrder;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    public ItemPlacement itemPlacementScript;

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

        if (Input.GetKeyDown(KeyCode.Mouse0) && DetectMouse())
        {
            itemPlacementScript.PassPrefabToPlace(prefabToPlace, pickUpSprite, sortingOrder);
        }

    }
    
    private bool DetectMouse()
    {
        float xBounds = pickUpSprite.rect.width / 32f;
        float yBounds = pickUpSprite.rect.height / 32f;

        Vector2 startingPosition = (Vector2) transform.position;

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
