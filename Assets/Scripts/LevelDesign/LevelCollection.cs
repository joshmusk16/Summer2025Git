using System.Collections.Generic;
using UnityEngine;

public class LevelCollection : MonoBehaviour
{
    [Header("Level Management")]
    [SerializeField] private List<LevelData> levels = new List<LevelData>();

    [Header("References")]
    private TileGrid tileGrid;
    private GameObject player;
    private PlayerLogic playerLogic;
    private PlayerTargeting playerTargeting;

    [Header("Spawn Dummies")]
    [SerializeField] private GameObject dummy;
    [SerializeField] private int amountOfDummies;

    private void FindDependencies()
    {
        if(tileGrid == null) tileGrid = FindObjectOfType<TileGrid>();
        if(playerLogic == null) playerLogic = FindObjectOfType<PlayerLogic>();
        if(playerTargeting == null) playerTargeting = FindObjectOfType<PlayerTargeting>();
        
        if(playerLogic != null && player == null)
        {
            player = playerLogic.gameObject;            
        }
    }

    public void ResetRandomLevel()
    {
        FindDependencies();

        LoadRandomLevel();
        MovePlayerToRandomTile();
        SpawnDummies(amountOfDummies);
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

    public void MovePlayerToRandomTile()
    {
        if (player == null)
        {
            return;
        }

        if (tileGrid == null || tileGrid.tiles.Count == 0)
        {
            return;
        }

        List<GameObject> validTiles = new List<GameObject>();

        foreach (GameObject tile in tileGrid.tiles)
        {
            TilePrefab tileScript = tile.GetComponent<TilePrefab>();
            if (tileScript != null)
            {
                if (tileScript.state != 0)
                {
                    validTiles.Add(tile);
                }
            }
        }

        if (validTiles.Count == 0)
        {
            return;
        }

        GameObject randomTile = validTiles[Random.Range(0, validTiles.Count)];
        Vector2 randomTilePos = randomTile.transform.position;
        playerLogic.RecordPlayerTilePosition();
        playerTargeting.ResetTargetingOrigin(randomTilePos);
        Vector3 spawnPosition = randomTilePos;
        player.transform.position = spawnPosition;
    }

    public void SpawnDummies(int amount)
    {
        if (dummy == null)
        {
            return;
        }

        if (tileGrid == null || tileGrid.tiles.Count == 0)
        {
            return;
        }
    
        List<GameObject> validTiles = new List<GameObject>();

        foreach (GameObject tile in tileGrid.tiles)
        {
            TilePrefab tileScript = tile.GetComponent<TilePrefab>();
            if (tileScript != null)
            {
                if (tileScript.state != 0)
                {
                    validTiles.Add(tile);
                }
            }
        }

        for (int i = 0; i < amount; i++)
        {
            if (validTiles.Count == 0) break;

            int randomIndex = Random.Range(0, validTiles.Count);
            GameObject selectedTile = validTiles[randomIndex];

            GameObject newDummy = Instantiate(dummy, selectedTile.transform.position, Quaternion.identity);
            RoundManager.RegisterEnemy(newDummy);

            tileGrid.ChangeObjectOnTileState(selectedTile, true);
            selectedTile.GetComponent<TilePrefab>().objectOnTile = newDummy;
            validTiles.RemoveAt(randomIndex);
        }
    }
}