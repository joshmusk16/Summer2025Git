using System.Collections.Generic;
using UnityEngine;

public class LevelCollection : MonoBehaviour
{
    [Header("Level Management")]
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    
    [Header("References")]
    private TileGrid tileGrid;

    void Start()
    {
        tileGrid= FindObjectOfType<TileGrid>();
        LoadRandomLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadRandomLevel();
        }
    }

    public void LoadRandomLevel()
    {
        if (levels.Count == 0)
        {
            Debug.LogWarning("No levels in collection!");
            return;
        }

        if (tileGrid == null)
        {
            Debug.LogError("TileGrid reference not found!");
            return;
        }

        int randomIndex = Random.Range(0, levels.Count);
        LevelData randomLevel = levels[randomIndex];

        Debug.Log($"Loading random level: {randomLevel.levelName} (Index: {randomIndex})");
        tileGrid.LoadLevel(randomLevel);
    }
}