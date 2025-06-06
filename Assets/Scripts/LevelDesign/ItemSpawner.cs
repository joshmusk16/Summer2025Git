using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public Sprite objectSprite;
    public GameObject objectToPlace;
    public ItemPlacement itemPlacementScript;

    public MouseTracker mouse;

    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = objectSprite;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && MouseDetected())
        {
            itemPlacementScript.InsertHeldItem(objectToPlace);
        }
    }

    bool MouseDetected()
        {
            float xBounds = objectSprite.rect.width / 32f;
            float yBounds = objectSprite.rect.height / 32f;

            if (mouse.worldPosition.x > transform.position.x - xBounds
            && mouse.worldPosition.x < transform.position.x + xBounds
            && mouse.worldPosition.y > transform.position.y - yBounds
            && mouse.worldPosition.y < transform.position.y + yBounds)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
}
