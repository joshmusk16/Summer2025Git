using UnityEngine;

public class TilePrefab : MonoBehaviour
{
    [Header("Tile Info")]
    public Sprite[] tiles = new Sprite[2];
    public int state = 0;

    public bool editable = true;
    public GameObject objectOnTile = null;

    public GameObject cliffPrefab;
    public GameObject cliff;

    public void DrawTile(int tileState)
    {
        state = tileState;
        gameObject.GetComponent<SpriteRenderer>().sprite = tiles[tileState];
    }

    public void DrawCliff()
    {
        if (cliff == null)
        {
            cliff = Instantiate(cliffPrefab, (Vector2)transform.position - new Vector2(0, 0.59375f), Quaternion.identity, transform);
            cliff.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        }
    }

    public void DestroyCliff()
    {
        Destroy(cliff);
    }

}
