using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    private TileGrid tileGrid;
    private MouseTracker mouseTracker;

    private Vector2 lastHoveredAttack = new(-1, -1);
    private Vector2 lastHoveredDefense = new(-1, -1);
    private Vector2 lastHoveredDash = new(-1, -1);

    private Vector2 cachedAttackTile;
    private Vector2 cachedDefenseTile;
    private Vector2 cachedDashTile;

    private Vector2 targetingOrigin;

    public GameObject attackCursor;
    public GameObject defenseCursor;
    public GameObject dashCursor;

    public int attackTargetingRange = 0;
    public int defenseTargetingRange = 0;
    public int dashTargetingRange = 0;

    private const int DEFAULT_TARGETING_RANGE = 4;

    void Start()
    {
        tileGrid = FindObjectOfType<TileGrid>();
        mouseTracker = FindObjectOfType<MouseTracker>();

        dashTargetingRange = DEFAULT_TARGETING_RANGE;

        targetingOrigin = transform.position;
    }

    void Update()
    {
        if (attackTargetingRange > 0)
        {
            attackCursor.transform.position = Vector2.Lerp(attackCursor.transform.position, SelectedTile(attackTargetingRange, ProgramType.Attack), Time.deltaTime * 20f); 
        }

        if(defenseTargetingRange > 0)
        {
            defenseCursor.transform.position = Vector2.Lerp(defenseCursor.transform.position, SelectedTile(defenseTargetingRange, ProgramType.Defense), Time.deltaTime * 20f);
        }

        if (dashTargetingRange > 0)
        {
            dashCursor.transform.position = Vector2.Lerp(dashCursor.transform.position, SelectedTile(dashTargetingRange, ProgramType.Dash), Time.deltaTime * 20f);   
        }
    }

    //Called in ProgramListData in DrawNewHand()
    public void InitializeTargetingValue(Program firstProgram)
    {
        ChangeTargetingRange(firstProgram.targetingRange, firstProgram.programType);
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

    public Vector2 SelectedTile(int range = 0, ProgramType programType = ProgramType.Attack)
    {
        if (tileGrid.tiles.Count == 0)
        {
            ref Vector2 cachedTileEmpty = ref GetCacheForType(programType);
            cachedTileEmpty = mouseTracker.worldPosition;
            return cachedTileEmpty;
        }

        Vector2 hoveredGridPos = WorldToGridPosition(mouseTracker.worldPosition);

        ref Vector2 cachedTile = ref GetCacheForType(programType);
        ref Vector2 lastHovered = ref GetLastHoveredForType(programType);

        // Return cached value if nothing changed
        if (hoveredGridPos == lastHovered && range == GetRangeForType(programType))
        {
            return cachedTile;
        }

        lastHovered = hoveredGridPos;

        // Check grid bounds
        bool insideGrid =
            hoveredGridPos.x >= 0 &&
            hoveredGridPos.y >= 0 &&
            hoveredGridPos.x < tileGrid.gridWidth &&
            hoveredGridPos.y < tileGrid.gridHeight;

        if (insideGrid)
        {
            TilePrefab tileScript =
                tileGrid.tileGrid[(int)hoveredGridPos.x, (int)hoveredGridPos.y]
                .GetComponent<TilePrefab>();

            bool tileValid = tileScript.state == 1;
            bool inRange = range <= 0 || IsWithinRange(hoveredGridPos, range);

            if (tileValid && inRange)
            {
                cachedTile = tileGrid
                    .tileGrid[(int)hoveredGridPos.x, (int)hoveredGridPos.y]
                    .transform.position;

                return cachedTile;
            }
        }

        // Fallbacks
        if (range > 0)
        {
            cachedTile = GetClosestInRangeTile(range);
            return cachedTile;
        }

        cachedTile = mouseTracker.worldPosition;
        return cachedTile;
    }

    private ref Vector2 GetCacheForType(ProgramType programType)
    {
        switch (programType)
        {
            case ProgramType.Attack:
                return ref cachedAttackTile;
            case ProgramType.Defense:
                return ref cachedDefenseTile;
            case ProgramType.Dash:
                return ref cachedDashTile;
            default:
                return ref cachedAttackTile;
        }
    }

    private int GetRangeForType(ProgramType programType)
    {
        switch (programType)
        {
            case ProgramType.Attack:
                return attackTargetingRange;
            case ProgramType.Defense:
                return defenseTargetingRange;
            case ProgramType.Dash:
                return dashTargetingRange;
            default:
                return 0;
        }
    }

    private ref Vector2 GetLastHoveredForType(ProgramType programType)
    {
        switch (programType)
        {
            case ProgramType.Attack:  
                return ref lastHoveredAttack;
            case ProgramType.Defense: 
                return ref lastHoveredDefense;
            case ProgramType.Dash:    
                return ref lastHoveredDash;
            default:                  
                return ref lastHoveredAttack;
        }
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

    public Vector2 ProgressTargetingOrigin(ProgramType programType)
    {
        if(programType == ProgramType.Attack)
        {
            targetingOrigin = SelectedTile(attackTargetingRange);   
        }
        else if(programType == ProgramType.Defense)
        {
            targetingOrigin = SelectedTile(defenseTargetingRange); 
        }
        else if(programType == ProgramType.Dash)
        {
            targetingOrigin = SelectedTile(dashTargetingRange); 
        }

        return targetingOrigin;
    }

    public void ChangeTargetingRange(int newRange, ProgramType programType)
    {
        if(newRange < 0) return;

        //Debug.Log($"ChangeTargetingRange called: range={newRange}, type={programType}");

        GameObject cursor = null;

        if(programType == ProgramType.Attack)
        {
            attackTargetingRange = newRange;
            cursor = attackCursor; 
        }
        else if(programType == ProgramType.Defense)
        {
            defenseTargetingRange = newRange;
            cursor = defenseCursor;
        }
        else if(programType == ProgramType.Dash)
        {
            dashTargetingRange = newRange; 
            cursor = dashCursor;
        }

        if(cursor != null)
        {
            if(newRange == 0)
            {
                cursor.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                cursor.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
}
