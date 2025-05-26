using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;

    public GameObject tilePrefab;

    private List<GameObject> tiles = new List<GameObject>();
    private GameObject[,] tileGrid;

    private readonly float tileWidth = 2f;
    private readonly float tileHeight = 1.1875f;

    public Slider gridWidthSlider;
    public Slider gridHeightSlider;
    public Button generateGridButton;

    void Update()
    {
        if (generateGridButton.activated)
        {
            gridWidth = gridWidthSlider.sliderCurrentValue;
            gridHeight = gridHeightSlider.sliderCurrentValue;
            GenerateGrid();
            generateGridButton.activated = false;
        }
    }


    public void GenerateGrid()
    {
        foreach (GameObject tile in tiles)
        {
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

}
