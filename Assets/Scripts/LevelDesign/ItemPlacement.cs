using UnityEngine;

public class ItemPlacement : MonoBehaviour
{
    public GameObject devItem;

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

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

        devItem.transform.position = Vector2.Lerp(devItem.transform.position, PlacementPosition(), Time.deltaTime * 10f);

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
                return tileGrid.tileGrid[(int)temp.x, (int)temp.y].transform.position;
            }
            else
            {
                return mouseWorldPosition;
            }
        }
        else
        {
            return mouseWorldPosition;
        }
    }
}
