using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    private TileGrid tileGrid;
    private MouseTracker mouseTracker;
    private TilePrefab nearestTileScript;
    public GameObject debug;

    void Start()
    {
        tileGrid = FindObjectOfType<TileGrid>();
        mouseTracker = FindObjectOfType<MouseTracker>();
    }

    void Update()
    {
        Debug.Log(SelectedTile(3));
        debug.transform.position = SelectedTile(3);
    }

    public Vector2 SelectedTile(int range = 0)
    {
        if (tileGrid.tiles.Count != 0)
        {
            Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
            Vector2 temp = mouseTracker.worldPosition - (Vector2)tileGrid.tileGrid[0, 0].transform.position + correction;
            temp = new Vector2(
                Mathf.FloorToInt(temp.x / tileGrid.tileWidth), 
                Mathf.FloorToInt(-temp.y / tileGrid.tileHeight)
            );

            if (temp.x < tileGrid.gridWidth && temp.y < tileGrid.gridHeight && temp.x >= 0 && temp.y >= 0)
            {
                nearestTileScript = tileGrid.tileGrid[(int)temp.x, (int)temp.y].GetComponent<TilePrefab>();
                if (nearestTileScript.state == 1)
                {
                    if (range > 0 && !IsWithinRange(temp, range))
                    {
                        return GetClosestInRangeTile(range);
                    }
                    return tileGrid.tileGrid[(int)temp.x, (int)temp.y].transform.position;
                }
                else
                {
                    // Tile is not valid (state != 1), find closest valid in-range tile
                    if (range > 0)
                    {
                        return GetClosestInRangeTile(range);
                    }
                    return mouseTracker.worldPosition;
                }
            }
            else if (range > 0)
            {
                // Mouse is outside grid bounds, find closest in-range tile
                return GetClosestInRangeTile(range);
            }
        }
        nearestTileScript = null;
        return mouseTracker.worldPosition;
    }

    private bool IsWithinRange(Vector2 targetGridPos, int range)
    {
        Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
        Vector2 playerGridPos = (Vector2)transform.position - (Vector2)tileGrid.tileGrid[0, 0].transform.position + correction;
        playerGridPos = new Vector2(
            Mathf.FloorToInt(playerGridPos.x / tileGrid.tileWidth), 
            Mathf.FloorToInt(-playerGridPos.y / tileGrid.tileHeight)
        );

        float distance = Mathf.Max(Mathf.Abs(targetGridPos.x - playerGridPos.x), 
                         Mathf.Abs(targetGridPos.y - playerGridPos.y));
        
        return distance <= range;
    }

    private Vector2 GetClosestInRangeTile(int range)
    {
    // Get player's grid position
    Vector2 correction = new(tileGrid.tileWidth / 2f, -tileGrid.tileHeight / 2f);
    Vector2 playerGridPos = (Vector2)transform.position - (Vector2)tileGrid.tileGrid[0, 0].transform.position + correction;
    playerGridPos = new Vector2(
        Mathf.FloorToInt(playerGridPos.x / tileGrid.tileWidth), 
        Mathf.FloorToInt(-playerGridPos.y / tileGrid.tileHeight)
    );

    // Calculate the bounding box for the range
    int minX = Mathf.Max(0, (int)playerGridPos.x - range);
    int maxX = Mathf.Min(tileGrid.gridWidth - 1, (int)playerGridPos.x + range);
    int minY = Mathf.Max(0, (int)playerGridPos.y - range);
    int maxY = Mathf.Min(tileGrid.gridHeight - 1, (int)playerGridPos.y + range);

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

    float closestDistance = float.MaxValue;
    Vector2 closestTilePosition = mouseTracker.worldPosition;

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
}
