using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    private TileGrid tileGrid;
    private MouseTracker mouseTracker;
    private TilePrefab nearestTileScript;
    public GameObject debug;

    private Vector2 lastHoveredTileGridPos = new Vector2(-1, -1);
    private Vector2 cachedSelectedTile;
    private Vector2 targetingOrigin;

    private int targetingRange;
    private const int DEFAULT_TARGETING_RANGE = 4;

    void Start()
    {
        tileGrid = FindObjectOfType<TileGrid>();
        mouseTracker = FindObjectOfType<MouseTracker>();
        targetingRange = DEFAULT_TARGETING_RANGE;
    }

    void Update()
    {
        Vector2 selectedTile = SelectedTile(targetingRange);
        debug.transform.position = Vector2.Lerp(debug.transform.position, selectedTile, Time.deltaTime * 20f);
        
        //To be removed later on when targeting system always recieves targeting origin from queue data collector properly
        targetingOrigin = transform.position;
    }

    private Vector2 WorldToGridPosition(Vector2 worldPosition)
    {
        Vector2 gridOrigin = (Vector2)tileGrid.tileGrid[0, 0].transform.position;
        Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
        Vector2 temp = worldPosition - gridOrigin + correction;
        
        return new Vector2(
            Mathf.FloorToInt(temp.x / tileGrid.tileWidth),
            Mathf.FloorToInt(-temp.y / tileGrid.tileHeight)
        );
    }

    public Vector2 SelectedTile(int range = 0)
    {
        if (tileGrid.tiles.Count == 0)
        {
            nearestTileScript = null;
            lastHoveredTileGridPos = new Vector2(-1, -1);
            return mouseTracker.worldPosition;
        }

        Vector2 temp = WorldToGridPosition(mouseTracker.worldPosition);

        if (temp == lastHoveredTileGridPos)
        {
            return cachedSelectedTile; // Return cached result
        }

        // Mouse is over a new tile, update tracking
        lastHoveredTileGridPos = temp;

        //Check that temp's position is in the grid's bounds
        if (temp.x < tileGrid.gridWidth && temp.y < tileGrid.gridHeight && temp.x >= 0 && temp.y >= 0)
        {
            nearestTileScript = tileGrid.tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
            if (nearestTileScript.state == 1)
            {
                if (range > 0 && !IsWithinRange(temp, range))
                {
                    cachedSelectedTile = GetClosestInRangeTile(range);
                    return cachedSelectedTile;
                }
                cachedSelectedTile = tileGrid.tileGrid[(int)temp.x, (int)temp.y].transform.position;
                return cachedSelectedTile;
            }
            else
            {
                // Tile is not valid (state != 1), find closest valid in-range tile
                if (range > 0)
                {
                    cachedSelectedTile = GetClosestInRangeTile(range);
                    return cachedSelectedTile;
                }
                cachedSelectedTile = mouseTracker.worldPosition;
                return cachedSelectedTile;
            }
        }
        else if (range > 0)
        {
            // Mouse is outside grid bounds, find closest in-range tile
            cachedSelectedTile = GetClosestInRangeTile(range);
            return cachedSelectedTile;
        }

        nearestTileScript = null;
        cachedSelectedTile = mouseTracker.worldPosition;
        return cachedSelectedTile;
    }

    private bool IsWithinRange(Vector2 targetGridPos, int range)
    {
        Vector2 playerGridPos = WorldToGridPosition(targetingOrigin);

        float distance = Mathf.Max(
                Mathf.Abs(targetGridPos.x - playerGridPos.x), 
                Mathf.Abs(targetGridPos.y - playerGridPos.y));
        
        return distance <= range;
    }

    private Vector2 GetClosestInRangeTile(int range)
    {
    Vector2 playerGridPos = WorldToGridPosition(targetingOrigin);

    int playerX = (int)playerGridPos.x;
    int playerY = (int)playerGridPos.y;

    // Calculate the bounding box for the range
    int minX = Mathf.Max(0, playerX - range);
    int maxX = Mathf.Min(tileGrid.gridWidth - 1, playerX + range);
    int minY = Mathf.Max(0, playerY - range);
    int maxY = Mathf.Min(tileGrid.gridHeight - 1, playerY + range);

    float closestDistance = float.MaxValue;
    Vector2 closestTilePosition = mouseTracker.worldPosition;

    // Step 1: Build array of valid tiles within range
    System.Collections.Generic.List<GameObject> validTilesInRange = new();
    
    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            // Check if tile is valid (state == 1)
            TilePrefab tileScript = tileGrid.tileGrid[x, y].GetComponent<TilePrefab>();
            if (tileScript.state == 1)
            {
                validTilesInRange.Add(tileGrid.tileGrid[x, y]);
            }
        }
    }

    // Step 2: Find closest tile to mouse from the filtered array
    if (validTilesInRange.Count == 0)
    {
        return mouseTracker.worldPosition;
    }

    foreach (GameObject tile in validTilesInRange)
    {
        Vector2 tileWorldPos = tile.transform.position;
        float distanceToMouse = Vector2.Distance(tileWorldPos, mouseTracker.worldPosition);
        
        if (distanceToMouse < closestDistance)
        {
            closestDistance = distanceToMouse;
            closestTilePosition = tileWorldPos;
        }
    }

    return closestTilePosition;
    }

    public Vector2 ProgressTargetingOrigin()
    {
        targetingOrigin = SelectedTile(targetingRange);
        return targetingOrigin;
    }

    public void ChangeTargetingRange(int newRange)
    {
        if(newRange <= 0) return;
        targetingRange = newRange;
    }
}
