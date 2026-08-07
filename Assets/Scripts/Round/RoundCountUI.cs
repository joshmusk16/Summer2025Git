using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoundCountUI : MonoBehaviour
{

public GameObject emptyRoundUI;
public GameObject emptyParent;
private GameObject roundUIParent;

private List<GameObject> roundUIObjects = new();
private List<SpriteRenderer> roundUISpriteRenderers = new();

public Sprite activeSprite;
public Sprite inactiveSprite;

private const float Y_OFFSET = 2.05f;
private const float SPACING = 12f;
private const float UI_SCALE = 1.25f;
private const float UI_PPU = 16f;
private const int SORTING_ORDER = 100;

void Update()
{
    if(Input.GetKeyDown(KeyCode.Y))
    {
        GenerateRoundUI(3);
    }

    if(Input.GetKeyDown(KeyCode.R))
    {
        IncrementRoundUI();
    }                
}

    private void GenerateRoundUI(int amountOfUI)
{
    DestroyUIObjects();

    roundUIParent = Instantiate(emptyParent, gameObject.transform);
    roundUIParent.name = "RoundCountUI Parent";
    Vector2 offset = Vector2.zero;

    for(int i = 0; i < amountOfUI; i++)
    {
        GameObject newRoundUI = Instantiate(emptyRoundUI, (Vector2)gameObject.transform.position + offset, 
        Quaternion.identity, gameObject.transform);
        newRoundUI.transform.localScale = Vector3.one * UI_SCALE;
        newRoundUI.name = "RoundCountUI Element";
        roundUIObjects.Add(newRoundUI);

        SpriteRenderer newUISpriteRenderer = newRoundUI.GetComponent<SpriteRenderer>();
        newUISpriteRenderer.sortingOrder = SORTING_ORDER - i;
        roundUISpriteRenderers.Add(newUISpriteRenderer);

        offset += new Vector2(SPACING / UI_PPU * UI_SCALE, 0);
    }

    float xValue = (roundUIObjects[^1].transform.position.x - roundUIObjects[0].transform.position.x) / 2f;
    float yValue = roundUIObjects[0].transform.position.y;

    roundUIParent.transform.position = new(xValue, yValue);

    foreach(GameObject roundUI in roundUIObjects)
    {
        roundUI.transform.SetParent(roundUIParent.transform);
    }

    roundUIParent.transform.position = gameObject.transform.position + new Vector3(0, Y_OFFSET);
}

public void IncrementRoundUI()
{
    if(roundUISpriteRenderers.Count == 0 ||
    roundUISpriteRenderers[^1].sprite == activeSprite)
    {
        GenerateRoundUI(3);
        return;       
    }

    foreach(SpriteRenderer sr in roundUISpriteRenderers)
    {
        if(sr.sprite != activeSprite)
        {
            sr.sprite = activeSprite;
            return;
        }
    }
}

private void DestroyUIObjects()
{
    if(roundUIObjects.Count == 0) return;

    if(roundUISpriteRenderers.Count != 0)
    {
        foreach(SpriteRenderer sr in roundUISpriteRenderers)
        {
            Destroy(sr);
        }
        roundUISpriteRenderers.Clear();   
    }

    foreach(GameObject roundUI in roundUIObjects)
    {
        Destroy(roundUI);
    }
    roundUIObjects.Clear();

    if(roundUIParent != null) Destroy(roundUIParent);  
}

}
