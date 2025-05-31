using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public Sprite objectSprite;
    public GameObject objectToPlace;
    public ItemPlacement itemPlacementScript;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        gameObject.GetComponent<SpriteRenderer>().sprite = objectSprite;
    }

    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                Mathf.Abs(mainCamera.transform.position.z)));


        if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected())
        {
            itemPlacementScript.InsertHeldItem(objectToPlace);
        }
    }

    bool MouseDetected()
        {
            float xBounds = objectSprite.rect.width / 32f;
            float yBounds = objectSprite.rect.height / 32f;

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
}
