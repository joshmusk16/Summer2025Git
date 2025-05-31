using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    [Header("Tile Info")]
    public Sprite[] tiles = new Sprite[3];
    public Sprite cliffSprite;
    public int state = 1;

    public bool editable = true;
    public GameObject objectOnTile = null;
    public bool drawCliff = false;
    private GameObject cliff;

    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = tiles[state];
    }

    public void DrawCliff()
    {
        if (cliff == null)
        {
            cliff = Instantiate(cliff, Vector2.zero, Quaternion.identity, transform);
            cliff.AddComponent<SpriteRenderer>().sprite = cliffSprite;
            cliff.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        }
        else
        {
            Destroy(cliff);   
        }
    }
    
}
