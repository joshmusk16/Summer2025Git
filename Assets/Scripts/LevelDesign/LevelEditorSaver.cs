using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelEditorSaver : MonoBehaviour
{
    [Header("References")]
    private TileGrid tileGrid;

    [Header("Save Settings")]
    [SerializeField] private string saveFolderPath = "Assets/Scripts/LevelDesign/Levels";
    [SerializeField] private string defaultLevelName = "CustomLevel";
    [SerializeField] private int nextLevelNumber = 1;

    [Header("Runtime Controls")]
    [SerializeField] private KeyCode saveKey;

    void Start()
    {
        tileGrid = FindObjectOfType<TileGrid>();
    }

    void Update()
    {
        if (Input.GetKeyDown(saveKey))
        {
            SaveCurrentLevel(defaultLevelName, nextLevelNumber);
        }
    }

    public void SaveCurrentLevel(string levelName, int levelNumber)
    {
#if UNITY_EDITOR
        if (tileGrid == null)
        {
            return;
        }

        LevelData newLevel = tileGrid.ExportToLevelData(levelName, levelNumber);

        if (newLevel == null)
        {
            Debug.LogError("Failed to export level data");
            return;
        }

        // Checking the save folder exists
        if (!AssetDatabase.IsValidFolder(saveFolderPath))
        {
            Debug.LogError("Failed to find folder path");
            return;
        }

        // Generate unique filename
        string fileName = $"{levelName}.asset";
        string fullPath = $"{saveFolderPath}/{fileName}";
        
        // If file already exists, add number suffix
        int counter = 1;
        while (AssetDatabase.LoadAssetAtPath<LevelData>(fullPath) != null)
        {
            fileName = $"{levelName}_{counter}.asset";
            fullPath = $"{saveFolderPath}/{fileName}";
            counter++;
        }

        // Create and save the asset
        AssetDatabase.CreateAsset(newLevel, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select the new asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
    }
#endif
}