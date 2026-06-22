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

public List<TurnUISettings> turnUISettings = new List<TurnUISettings>();
public List<GameObject> turnUIObjects = new List<GameObject>();
private List<Vector3> turnUIPositions = new List<Vector3>();

private const float UI_PPU = 16f;
private const int SORTING_ORDER = 100;
private const float LERP_SPEED = 25f;

private Vector2 spriteOffsetVector;
private float centerOnXOffset;

void Awake()
{
    if(turnUISprite != null) turnUIWidth = GetTightWidth(turnUISprite);
    Debug.Log("Sprite Width is" + turnUIWidth);

    spriteOffsetVector = GetTightBottomLeftOffset(turnUISprite);

    InitializeTurnUI(5); //for debugging only
}

//Update for debugging only
void Update()
{
    if (Input.GetKeyDown(KeyCode.Y))
    {
        ProgressTurnUI(Random.Range(1, 6));
    }

    if (Input.GetKeyDown(KeyCode.U))
    {
        RemoveTurnUI(1);
    }
}

public void InitializeTurnUI(int amount)
{
    GenerateTurnUIPositions(amount);
    InstantiateTurnUIObjects(amount);
    AssignTurnUIPositions();
}

public void ProgressTurnUI(int amount)
{
    if(turnUIObjects.Count == 0) return;

    GenerateTurnUIPositions(amount);
    InstantiateTurnUIObjects(amount);
    LerpTurnUIToPositions();
}


public void InstantiateTurnUIObjects(int numberOfTurns)
{
    DestroyTurnUIObjects();

    float scale = turnUISettings[numberOfTurns - 1].uiScale;
    
    for(int i = 0; i < numberOfTurns; i++)
    {
        Vector3 worldPos = transform.TransformPoint(turnUIPositions[i]);
        GameObject turnUI = Instantiate(emptyTurnUIPrefab, worldPos, Quaternion.identity, gameObject.transform);
        turnUI.transform.localScale = new Vector2(scale, scale);
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

    float scale = turnUISettings[turnUIObjects.Count - 1].uiScale;

    for(int i = 0; i < turnUIObjects.Count; i++)
    {
        turnUIObjects[i].transform.localPosition = turnUIPositions[i];
        turnUIObjects[i].transform.localScale = new Vector2(scale, scale);
    }
}

public void LerpTurnUIToPositions()
{
    if(turnUIObjects.Count != turnUIPositions.Count) return;

    float scale = turnUISettings[turnUIObjects.Count - 1].uiScale;

    for(int i = 0; i < turnUIObjects.Count; i++)
    {
        Vector3 worldPos = transform.TransformPoint(turnUIPositions[i]);
        turnUIObjects[i].GetComponent<LerpUIHandler>().LocationLerp(worldPos, LERP_SPEED);
        turnUIObjects[i].GetComponent<LerpUIHandler>().ScaleLerp(new Vector2(scale, scale), LERP_SPEED);
    }
}

public void RemoveTurnUI(int amount)
{
    int removeCount = Mathf.Min(amount, turnUIObjects.Count - 1);

    for(int i = 0; i < removeCount; i++)
    {
        GameObject objToDestroy = turnUIObjects[0];
        turnUIObjects.RemoveAt(0);
        Destroy(objToDestroy);
    }

    int newAmount = turnUIObjects.Count;

    GenerateTurnUIPositions(newAmount);
    LerpTurnUIToPositions();
}

public void GenerateTurnUIPositions(int numberOfTurns)
{
    if(turnUIWidth == 0 && turnUISprite != null) turnUIWidth = GetTightWidth(turnUISprite);

    turnUIPositions.Clear();

    TurnUISettings settings = turnUISettings[numberOfTurns - 1];
    float scale = settings.uiScale;
    float horiztonalOffset = settings.uiHorizontalOffset / UI_PPU;
    int firstLineTurnUIAmount;
    Vector3 offset = -(spriteOffsetVector * scale);

    if(turnUISettings[numberOfTurns - 1].isTwoLines)
    {
        firstLineTurnUIAmount = Mathf.CeilToInt(numberOfTurns / 2f);
    }
    else
    {
        firstLineTurnUIAmount = numberOfTurns;
    }

    centerOnXOffset = ((firstLineTurnUIAmount * turnUIWidth) + (horiztonalOffset * (firstLineTurnUIAmount - 1))) / 2f * scale;

    int index = 0;

    for (int i = 0; i < numberOfTurns; i++)
    {

        if(i == firstLineTurnUIAmount)
        {
            offset = SecondLineStartLocation(numberOfTurns, settings) - (spriteOffsetVector * scale);
            index = 0;
        }

        turnUIPositions.Add(offset);
        offset += new Vector3(turnUIWidth + horiztonalOffset, 0) * scale;
        index++;
    }

    //Final offset to center everyPosition on the X axis
    for(int i = 0; i < turnUIPositions.Count; i++)
        {
            turnUIPositions[i] -= new Vector3(centerOnXOffset, 0 ,0);
        }
}

private Vector2 SecondLineStartLocation(int numberOfTurns, TurnUISettings turnUI)
{
    if(turnUI.isTwoLines == false) return Vector2.zero;

    int firstLineTurnUIAmount = Mathf.CeilToInt(numberOfTurns / 2f);
    int secondLineTurnUIAmount = numberOfTurns - firstLineTurnUIAmount;
    float scale = turnUI.uiScale;

    float verticalOffset = (turnUIWidth + (turnUI.uiVerticalOffset / UI_PPU)) * scale; 
    float horiztonalOffset = turnUI.uiHorizontalOffset / UI_PPU;


    float xLocation = centerOnXOffset
    - ((secondLineTurnUIAmount * turnUIWidth + (horiztonalOffset * (secondLineTurnUIAmount - 1))) / 2f * scale);


    return new Vector2(xLocation, -verticalOffset);
}

}