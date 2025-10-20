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
    [Tooltip("0 = inactive/empty, 1 = active/solid")]
    public int[] tileStates;

    // Validate that the level data is correct
    public bool IsValidLevel()
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

    [ContextMenu("Initialize Tile Array Size")]
    public void InitializeTileArraySize()
    {
        tileStates = new int[width * height];
        Debug.Log($"Initialized tile array for {width}x{height} grid ({tileStates.Length} tiles)");
    }
}
