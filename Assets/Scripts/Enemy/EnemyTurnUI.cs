using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnUI : SpriteSampling
{

[System.Serializable]
public struct TurnUISettings
{
    public float uiScale;
    public float uiHorizontalOffset;
    public float uiVerticalOffset;
    public bool isTwoLines;
}

public GameObject emptyTurnUIPrefab;
public Sprite turnUISprite;
private float turnUIWidth = 0;

public int debugAmount = 5;

public List<TurnUISettings> turnUISettings = new List<TurnUISettings>();
private List<Vector3> turnUIPositions = new List<Vector3>();
private List<GameObject> turnUIObjects = new List<GameObject>();

private const float UI_PPU = 16f;
private const int SORTING_ORDER = 100;

void Awake()
{
    if(turnUISprite != null) turnUIWidth = GetTightWidth(turnUISprite);
    Debug.Log("Sprite Width is" + turnUIWidth);
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.Y))
    {
        InstantiateTurnUIObjects(debugAmount);
        GenerateTurnUIPositions(debugAmount);
        AssignTurnUIPositions();
    }
}

public void InstantiateTurnUIObjects(int numberOfTurns)
{
    DestroyTurnUIObjects();
    
    for(int i = 0; i < numberOfTurns; i++)
    {
        GameObject turnUI = Instantiate(emptyTurnUIPrefab, Vector2.zero, Quaternion.identity, gameObject.transform);
        turnUI.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;
        turnUIObjects.Add(turnUI);
    }
}

public void DestroyTurnUIObjects()
{
    foreach(GameObject turnUI in turnUIObjects)
    {
        Destroy(turnUI);
    }

    turnUIObjects.Clear();
}

public void AssignTurnUIPositions()
{
    if(turnUIObjects.Count != turnUIPositions.Count) return;

    for(int i = 0; i < turnUIObjects.Count; i++)
    {
        turnUIObjects[i].transform.position = turnUIPositions[i];
    }
}

public void GenerateTurnUIPositions(int numberOfTurns)
{
    if(turnUIWidth == 0 && turnUISprite != null) turnUIWidth = GetTightWidth(turnUISprite);
    if(turnUIObjects.Count == 0) return;

    turnUIPositions.Clear();

    Vector3 offset = Vector2.zero;
    TurnUISettings settings = turnUISettings[numberOfTurns - 1];
    float scale = settings.uiScale;
    float horiztonalOffset = settings.uiHorizontalOffset;
    int firstLineTurnUIAmount;

    if(turnUISettings[numberOfTurns - 1].isTwoLines)
    {
        firstLineTurnUIAmount = Mathf.CeilToInt(numberOfTurns / 2f);
    }
    else
    {
        firstLineTurnUIAmount = numberOfTurns;
    }

    int index = 0;

    for (int i = 0; i < numberOfTurns; i++)
    {

        if(i == firstLineTurnUIAmount)
        {
            offset = SecondLineStartLocation(numberOfTurns, settings);
            index = 0;
        }

        turnUIPositions.Add(offset);
        offset += new Vector3(turnUIWidth + (horiztonalOffset / UI_PPU), 0) * scale;
        index++;
    }
}

private Vector2 SecondLineStartLocation(int numberOfTurns, TurnUISettings turnUI)
{
    if(turnUI.isTwoLines == false) return Vector2.zero;

    int firstLineTurnUIAmount = Mathf.CeilToInt(numberOfTurns / 2f);
    int secondLineTurnUIAmount = numberOfTurns - firstLineTurnUIAmount;
    float scale = turnUI.uiScale;

    float verticalOffset = (turnUIWidth + (turnUI.uiVerticalOffset / UI_PPU)) * scale; 
    float horiztonalOffset = turnUI.uiHorizontalOffset;

    float xLocation = (firstLineTurnUIAmount * turnUIWidth + (horiztonalOffset * (firstLineTurnUIAmount - 1))) / 2f 
    - (secondLineTurnUIAmount * turnUIWidth + (horiztonalOffset * (secondLineTurnUIAmount - 1))) / 2f;

    return new Vector2(xLocation, -verticalOffset);
}

}
