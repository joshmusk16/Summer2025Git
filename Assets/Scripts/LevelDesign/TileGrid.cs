using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;

    public GameObject tilePrefab;

    public List<GameObject> tiles = new List<GameObject>();
    public GameObject[,] tileGrid;
    private bool isEditable = true;

    public readonly float tileWidth = 2f;
    public readonly float tileHeight = 1.1875f;

    public Slider gridWidthSlider;
    public Slider gridHeightSlider;
    public Button generateGridButton;

    private GameObject nearestTile;
    private TilePrefab nearestTileScript;
    private int nearestTileX;
    private int nearestTileY;

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

        if (generateGridButton.activated)
        {
            gridWidth = gridWidthSlider.sliderCurrentValue;
            gridHeight = gridHeightSlider.sliderCurrentValue;
            GenerateGrid();
            generateGridButton.activated = false;
        }

        FindNearestTile();

        if (isEditable)
        {
            if (Input.GetKey(KeyCode.Mouse0) && nearestTile != null)
            {
                nearestTileScript.state = 0;
                //implement cliff drawing
            }

            if (Input.GetKey(KeyCode.Mouse1) && nearestTile != null)
            {
                nearestTileScript.state = 1;
                if (nearestTileScript.objectOnTile != null)
                {
                    Destroy(nearestTileScript.objectOnTile);
                }
            }
        }
    }

//==========================================================================================
//==========================================================================================

    public void GenerateGrid()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<TilePrefab>().objectOnTile != null)
            {
                Destroy(tile.GetComponent<TilePrefab>().objectOnTile);
            }
            Destroy(tile);
        }

        tiles.Clear();

        tileGrid = new GameObject[gridWidth, gridHeight];
        Vector2 startPosition = new Vector2(-gridWidth * tileWidth / 2f, gridHeight * tileHeight / 2f);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 tilePosition = startPosition + new Vector2(x * tileWidth, y * -tileHeight);

                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, gameObject.transform);
                tile.name = "Tile " + x + "," + y;
                tiles.Add(tile);
                tileGrid[x, y] = tile;
            }
        }
    }

    public void FindNearestTile()
    {
        if (tiles.Count != 0)
        {
            Vector2 correction = new(tileWidth / 2f, -tileHeight / 2f);
            Vector2 temp = mouseWorldPosition - (Vector2)tileGrid[0, 0].transform.position + correction;
            temp = new Vector2(Mathf.FloorToInt(temp.x / tileWidth), Mathf.FloorToInt(-temp.y / tileHeight));

            if (temp.x < gridWidth && temp.y < gridHeight && temp.x >= 0 && temp.y >= 0)
            {
                nearestTileX = (int)temp.x;
                nearestTileY = (int)temp.y;
                nearestTile = tileGrid[(int)temp.x, (int)temp.y];
                nearestTileScript = tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
                return;
            }
        }
        nearestTile = null;
        nearestTileScript = null;
        return;
    }

    public void EnableEditing()
    {
        isEditable = true;
    }

    public void DisableEditing()
    {
        isEditable = false;
    }

}
