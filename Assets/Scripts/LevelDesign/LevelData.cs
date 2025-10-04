using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;
    public int levelNumber;
    
    [Header("Grid Dimensions")]
    public int width;
    public int height;
    
    [Header("Tile States")]
    [Tooltip("0 = active/solid, 1 = inactive/empty")]
    public int[] tileStates;

    // Validate that the data is correct
    public bool IsValid()
    {
        if (tileStates == null)
        {
            Debug.LogError($"Level {levelName}: tileStates is null!");
            return false;
        }
        
        int expectedLength = width * height;
        if (tileStates.Length != expectedLength)
        {
            Debug.LogError($"Level {levelName}: Expected {expectedLength} tiles, got {tileStates.Length}");
            return false;
        }
        
        return true;
    }

    // Get tile state at specific grid position
    public int GetTileState(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return -1;
        
        return tileStates[y * width + x];
    }

    // Helper method to initialize/resize the array from inspector
    [ContextMenu("Initialize Tile Array")]
    public void InitializeTileArray()
    {
        tileStates = new int[width * height];
        Debug.Log($"Initialized tile array for {width}x{height} grid ({tileStates.Length} tiles)");
    }

    // Helper to fill with a pattern (useful for testing)
    [ContextMenu("Fill with Empty Border")]
    public void FillWithEmptyBorder()
    {
        if (tileStates == null || tileStates.Length != width * height)
        {
            InitializeTileArray();
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                // Make borders inactive (1), interior active (0)
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    tileStates[index] = 1;
                else
                    tileStates[index] = 0;
            }
        }
        Debug.Log("Filled with empty border pattern");
    }
}
