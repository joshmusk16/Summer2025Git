using UnityEngine;

public class ItemPlacement : MonoBehaviour
{
    [Header("Is An Item Held?")]
    public bool itemHeld = false;
    public GameObject heldObject;

    private TilePrefab nearestTileScript;
    private GameObject nearestTile;

    [Header("TileGrid Manager")]
    public TileGrid tileGrid;

    public MouseTracker mouse;

    void Update()
    {

        PlacementPosition();

        if (Input.GetKeyDown(KeyCode.Mouse0) && nearestTileScript != null
        && nearestTileScript.objectOnTile != null && itemHeld == false)
        {
            heldObject = nearestTileScript.objectOnTile;
            nearestTileScript.objectOnTile = null;
            itemHeld = true;
            tileGrid.DisableEditing();
        }

        if (itemHeld)
        {
            heldObject.transform.position = Vector2.Lerp(heldObject.transform.position, PlacementPosition(), Time.deltaTime * 10f);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && itemHeld)
        {
            if (nearestTileScript != null && nearestTileScript.objectOnTile == false && nearestTileScript.state == 0)
            {
                heldObject.transform.position = nearestTile.transform.position;
                nearestTileScript.objectOnTile = heldObject;
                heldObject.GetComponent<SpriteRenderer>().sortingOrder = nearestTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
            else if (nearestTileScript == null || nearestTileScript.objectOnTile == false)
            {
                Destroy(heldObject);
            }
            heldObject = null;
            itemHeld = false;
            tileGrid.EnableEditing();
        }
    }

    public Vector2 PlacementPosition()
    {
        if (tileGrid.tiles.Count != 0)
        {
            Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
            Vector2 temp = mouse.worldPosition - (Vector2)tileGrid.tileGrid[0, 0].transform.position + correction;
            temp = new Vector2(Mathf.FloorToInt(temp.x / tileGrid.tileWidth), Mathf.FloorToInt(-temp.y / tileGrid.tileHeight));

            if (temp.x < tileGrid.gridWidth && temp.y < tileGrid.gridHeight && temp.x >= 0 && temp.y >= 0)
            {
                nearestTile = tileGrid.tileGrid[(int)temp.x, (int)temp.y];
                nearestTileScript = tileGrid.tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
                if (nearestTileScript.state == 0)
                {
                    return tileGrid.tileGrid[(int)temp.x, (int)temp.y].transform.position;
                }
                else
                {
                    return mouse.worldPosition;
                }
            }
        }
        nearestTile = null;
        nearestTileScript = null;
        return mouse.worldPosition;
    }

    public void InsertHeldItem(GameObject itemToHold)
    {
        if (itemHeld == false)
        {
            heldObject = Instantiate(itemToHold, mouse.worldPosition, Quaternion.identity);
            tileGrid.DisableEditing();
            itemHeld = true;
        }
    }
}
