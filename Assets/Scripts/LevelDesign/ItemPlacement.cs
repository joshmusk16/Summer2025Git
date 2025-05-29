using UnityEngine;

public class ItemPlacement : MonoBehaviour
{
    [Header("Prefab Object and Sprites")]
    public GameObject emptyObject;
    public GameObject prefabToPlace;

    [Header("Is An Item Held?")]
    public bool itemHeld = false;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    private bool placementLocationFound = false;
    private TilePrefab nearestTile;
    private GameObject temp;

    [Header("TileGrid Manager")]
    public TileGrid tileGrid;

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

        if (Input.GetKey(KeyCode.Mouse0) && itemHeld)
        {
            temp.transform.position = Vector2.Lerp(temp.transform.position, PlacementPosition(), Time.deltaTime * 10f);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && itemHeld)
        {
            if (placementLocationFound && nearestTile.objectOnTile == false)
            {
                Instantiate(prefabToPlace, PlacementPosition(), Quaternion.identity);
                nearestTile.objectOnTile = true;
            }
            Destroy(temp);
            itemHeld = false;
            tileGrid.EnableEditing();
        }

    }

    public Vector2 PlacementPosition()
    {
        if (tileGrid.tiles.Count != 0)
        {
            Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
            Vector2 temp = mouseWorldPosition - (Vector2)tileGrid.tileGrid[0, 0].transform.position + correction;
            temp = new Vector2(Mathf.FloorToInt(temp.x / tileGrid.tileWidth), Mathf.FloorToInt(-temp.y / tileGrid.tileHeight));

            if (temp.x < tileGrid.gridWidth && temp.y < tileGrid.gridHeight && temp.x >= 0 && temp.y >= 0)
            {
                nearestTile = tileGrid.tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
                placementLocationFound = true;
                return tileGrid.tileGrid[(int)temp.x, (int)temp.y].transform.position;
            }
            else
            {
                nearestTile = null;
                placementLocationFound = false;
                return mouseWorldPosition;
            }
        }
        else
        {
            nearestTile = null;
            placementLocationFound = false;
            return mouseWorldPosition;
        }
    }

    public void PassPrefabToPlace(GameObject prefab, Sprite prefabSprite, int sortingOrder)
    {
        itemHeld = true;
        prefabToPlace = prefab;
        temp = Instantiate(emptyObject, PlacementPosition(), Quaternion.identity);
        temp.GetComponent<SpriteRenderer>().sprite = prefabSprite;
        temp.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        tileGrid.DisableEditing();
    }

}
