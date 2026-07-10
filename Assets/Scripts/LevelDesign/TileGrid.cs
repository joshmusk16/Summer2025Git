using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;
    public int gridLowestSortingLayer;

    public GameObject tilePrefab;

    public List<GameObject> tiles = new List<GameObject>();
    public Dictionary<GameObject, bool> objectOnTileStates = new(); //true = object on tile, false = object not on tile
    public GameObject[,] tileGrid;
    [SerializeField] private bool isEditable = false;

    public readonly float tileWidth = 2f;
    public readonly float tileHeight = 1.1875f;

    [Header("DevMenu UI References")]
    public Slider gridWidthSlider;
    public Slider gridHeightSlider;
    public Button generateGridButton;

    private GameObject nearestTile;
    private TilePrefab nearestTileScript;
    private int nearestTileX;
    private int nearestTileY;

    public MouseTracker mouse; 

    void Update()
    {
        if (generateGridButton.activated)
        {
            gridWidth = gridWidthSlider.sliderCurrentValue;
            gridHeight = gridHeightSlider.sliderCurrentValue;
            GenerateEmptyGrid();
            generateGridButton.activated = false;
        }

        FindNearestTile();

        if (isEditable)
        {
            if (Input.GetKey(KeyCode.Mouse0) && nearestTile != null)
            {
                nearestTileScript.DrawTile(1);
                if (nearestTileY == gridHeight - 1)
                {
                    nearestTileScript.DrawCliff();
                }
                else if (tileGrid[nearestTileX, nearestTileY + 1].GetComponent<TilePrefab>().state != 1)
                {
                    nearestTileScript.DrawCliff();
                }

                if (nearestTileY != 0 && tileGrid[nearestTileX, nearestTileY - 1].GetComponent<TilePrefab>().cliff != null)
                {
                    tileGrid[nearestTileX, nearestTileY - 1].GetComponent<TilePrefab>().DestroyCliff();
                }
            }

            if (Input.GetKey(KeyCode.Mouse1) && nearestTile != null)
            {
                nearestTileScript.DrawTile(0);
                if (nearestTileScript.objectOnTile != null)
                {
                    Destroy(nearestTileScript.objectOnTile);
                }

                if (nearestTileScript.cliff != null)
                {
                    Destroy(nearestTileScript.cliff);
                }

                if (nearestTileY != 0 && tileGrid[nearestTileX, nearestTileY - 1].GetComponent<TilePrefab>().cliff == null &&
                tileGrid[nearestTileX, nearestTileY - 1].GetComponent<TilePrefab>().state == 1)
                {
                    tileGrid[nearestTileX, nearestTileY - 1].GetComponent<TilePrefab>().DrawCliff();
                }
            }
        }
    }

    //==========================================================================================
    //==========================================================================================

    private void InitializeObjectOnTileStates()
    {
        objectOnTileStates.Clear();

        if(tiles.Count == 0) return;
 
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null)
            {
                objectOnTileStates[tiles[i]] = false;
            }
        }
    }

    public void ChangeObjectOnTileState(GameObject tile, bool newState)
    {
        if (objectOnTileStates.ContainsKey(tile))
        {
            objectOnTileStates[tile] = newState;
        }
    }

    public bool GetObjectOnTileState(GameObject tile)
    {
        if (!objectOnTileStates.ContainsKey(tile))
        {
            Debug.Log("Does not contain key");
            return true;
        }

        Debug.Log("Returned key" + objectOnTileStates[tile]);
        return objectOnTileStates[tile];
    }

    public void LoadLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("Cannot load null level data!");
            return;
        }

        if (!levelData.IsValidLevel())
        {
            Debug.LogError($"Level data '{levelData.levelName}' is invalid!");
            return;
        }

        Debug.Log($"Loading level: {levelData.levelName} ({levelData.width}x{levelData.height})");

        gridWidth = levelData.width;
        gridHeight = levelData.height;

        GenerateGridFromData(levelData.width, levelData.height, levelData.tileStates);
    }

    public LevelData ExportToLevelData(string levelName, int levelNumber)
    {
        if (tileGrid == null)
        {
            return null;
        }

        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
        newLevel.levelName = levelName;
        newLevel.levelNumber = levelNumber;
        newLevel.width = gridWidth;
        newLevel.height = gridHeight;
        newLevel.tileStates = new int[gridWidth * gridHeight];

        int index = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                newLevel.tileStates[index] = tileGrid[x, y].GetComponent<TilePrefab>().state;
                index++;
            }
        }

        Debug.Log($"Exported level: {levelName}");
        return newLevel;
    }

    public void GenerateEmptyGrid()
    {
        ClearGrid();

        tileGrid = new GameObject[gridWidth, gridHeight];
        Vector2 startPosition = new(-gridWidth * tileWidth / 2f, gridHeight * tileHeight / 2f);

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 tilePosition = startPosition + new Vector2(x * tileWidth, y * -tileHeight);

                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, gameObject.transform);
                tile.GetComponent<SpriteRenderer>().sortingOrder = gridLowestSortingLayer + y;
                tile.name = "Tile " + x + "," + y;
                tiles.Add(tile);
                tileGrid[x, y] = tile;
            }
        }
    }

    private void GenerateGridFromData(int gridWidth, int gridHeight, int[] stateData)
    {
        ClearGrid();

        tileGrid = new GameObject[gridWidth, gridHeight];
        Vector2 startPosition = new(-gridWidth * tileWidth / 2f, gridHeight * tileHeight / 2f);

        int dataIndex = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 tilePosition = startPosition + new Vector2(x * tileWidth, y * -tileHeight);

                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, gameObject.transform);
                tile.GetComponent<SpriteRenderer>().sortingOrder = gridLowestSortingLayer + y;
                tile.name = "Tile " + x + "," + y;

                // Set tile state from array
                TilePrefab tileScript = tile.GetComponent<TilePrefab>();
                int tileState = stateData[dataIndex];
                tileScript.DrawTile(tileState);

                // Apply visual changes based on state
                if (tileState == 1)
                {
                    // Active tile - check if needs cliff
                    if (y == gridHeight - 1)
                    {
                        tileScript.DrawCliff();
                    }
                }

                tiles.Add(tile);
                tileGrid[x, y] = tile;
                dataIndex++;
            }
        }

        // Second pass: draw cliffs for tiles that need them based on neighbors
        for (int y = 0; y < gridHeight - 1; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                TilePrefab currentTile = tileGrid[x, y].GetComponent<TilePrefab>();
                TilePrefab tileBelow = tileGrid[x, y + 1].GetComponent<TilePrefab>();

                if (currentTile.state == 1 && tileBelow.state != 1)
                {
                    currentTile.DrawCliff();
                }
            }
        }

        InitializeObjectOnTileStates();
    }

    public void FindNearestTile()
    {
        if (tiles.Count != 0)
        {
            Vector2 correction = new(tileWidth / 2f, -tileHeight / 2f);
            Vector2 temp = mouse.worldPosition - (Vector2)tileGrid[0, 0].transform.position + correction;
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

    public GameObject FindNearestTileToGameObject(GameObject obj)
    {
        if (tiles.Count != 0)
        {
            Vector2 correction = new(tileWidth / 2f, -tileHeight / 2f);
            Vector2 temp = (Vector2)obj.transform.position - (Vector2)tileGrid[0, 0].transform.position + correction;
            temp = new Vector2(Mathf.FloorToInt(temp.x / tileWidth), Mathf.FloorToInt(-temp.y / tileHeight));

            if (temp.x < gridWidth && temp.y < gridHeight && temp.x >= 0 && temp.y >= 0)
            {
                nearestTileX = (int)temp.x;
                nearestTileY = (int)temp.y;
                nearestTile = tileGrid[(int)temp.x, (int)temp.y];
                nearestTileScript = tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
                return nearestTile;
            }
        }

        return null;
    }

    public int GetTileDistanceBetweenObjects(GameObject objA, GameObject objB)
    {
        if (tiles.Count == 0) return 0;

        Vector2 correction = new(tileWidth / 2f, -tileHeight / 2f);
        Vector2 origin = (Vector2)tileGrid[0, 0].transform.position;

        Vector2 tempA = (Vector2)objA.transform.position - origin + correction;
        tempA = new Vector2(Mathf.FloorToInt(tempA.x / tileWidth), Mathf.FloorToInt(-tempA.y / tileHeight));

        Vector2 tempB = (Vector2)objB.transform.position - origin + correction;
        tempB = new Vector2(Mathf.FloorToInt(tempB.x / tileWidth), Mathf.FloorToInt(-tempB.y / tileHeight));

        return Mathf.Max(Mathf.Abs((int)tempB.x - (int)tempA.x), Mathf.Abs((int)tempB.y - (int)tempA.y));
    }

    public void ClearGrid()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<TilePrefab>().objectOnTile)
            {
                Destroy(tile.GetComponent<TilePrefab>().objectOnTile);
            }
            Destroy(tile);
        }

        tiles.Clear();
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
