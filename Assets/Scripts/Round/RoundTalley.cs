using System.Collections.Generic;
using UnityEngine;

public class RoundTalley : MonoBehaviour
{

public GameObject backgroundCanvasPrefab;
private GameObject backgroundCanvas;
private RectTransform backgroundCanvasRect;
private const float CANVAS_BUFFER = 0.6f;
private const float CANVAS_WIDTH = 12f;

public GameObject emptyObject;
public GameObject emptyTextElement;

private GameObject roundTalleyParent;
private List<string> lines = new();
private List<TextElement> lineTextElements = new();

private const float FIRST_LINE_SCALE = 1f;
private const float LOWER_LINES_SCALE = 0.75f;
private const float SPACE_BETWEEN_LINES = 1f;

private Vector3 offsetVector = Vector3.zero;

private bool isAnimating = false;
private float currentTime = 0;
private const float TIME_BETWEEN_LINE_GENERATION = 0.3f; //in seconds
private int lineIndex = 0;

private int moneyRewardPot = 10;
private int moneyRewardPotMaximum = 20;
private int moneyRewardPotMinimum = -10;
private int moneyReward = 0;
private int combosDropped = 0;
private int secondsLost = 0;

private MoneyLogic moneyLogic;

private void FindDependencies()
{
    if(moneyLogic == null) moneyLogic = FindObjectOfType<MoneyLogic>();
}

private void Update()
{
    if (Input.GetKeyDown(KeyCode.D)) //Keycode for debugging only
    {
        StartTalleyEvent();
    }

    if (isAnimating)
    {
        currentTime += Time.deltaTime;
        if(currentTime >= TIME_BETWEEN_LINE_GENERATION)
        {
            if(lineIndex < lines.Count)
            {
                if(lineIndex == 0)
                {
                    GenerateCanvas();
                }
                
                GenerateSingleLine(lines[lineIndex], offsetVector);
        
                backgroundCanvasRect.sizeDelta = new Vector2(CANVAS_WIDTH, (SPACE_BETWEEN_LINES * (lineIndex + 1)) + CANVAS_BUFFER);

                currentTime = 0;
                lineIndex++;
                offsetVector -= new Vector3(0, SPACE_BETWEEN_LINES, 0);

                if(lineIndex >= lines.Count)
                {
                    offsetVector = gameObject.transform.position;
                    lineIndex = 0;
                    GiveMoneyReward();
                    isAnimating = false;
                }
            }
        }
    }
}

private void StartTalleyEvent()
{
    DestroyLineObjects();
    AssignTalleyTextElements();
    StartTalleyAnimation();
}

private void AssignTalleyTextElements()
{
    if(isAnimating) return;

    ClearTalleyQueue();

    lines.Add(moneyRewardPot.ToString());
    lines.Add("-" + combosDropped.ToString() + " COMBOS DROPPED");
    lines.Add("-" + secondsLost.ToString() + " SECONDS LOST");
    lines.Add("---------------------------");
    
    lines.Add(GetMoneyRewardString());
}

private void StartTalleyAnimation()
{
    offsetVector = gameObject.transform.position;
    lineIndex = 0;
    currentTime = 0;
    isAnimating = true;  
}

private void GenerateSingleLine(string text, Vector3 position)
{
    if(backgroundCanvas == null || roundTalleyParent == null) return;

    GameObject line = Instantiate(emptyTextElement, roundTalleyParent.transform);
    line.name = "Line " + lineIndex;
    TextElement textElement = line.GetComponent<TextElement>();
    lineTextElements.Add(textElement);
    
    textElement.textInput = text;
    textElement.GenerateTextElement();    
    textElement.MoveTextParent(position -= new Vector3(-CANVAS_BUFFER, CANVAS_BUFFER));
    line.transform.SetParent(roundTalleyParent.transform);
}

private void GenerateCanvas()
{
    if(backgroundCanvas != null)
    {
        backgroundCanvasRect = null;
        Destroy(backgroundCanvas);       
    }

    if(roundTalleyParent != null) Destroy(roundTalleyParent);

    backgroundCanvas = Instantiate(backgroundCanvasPrefab, gameObject.transform);
    backgroundCanvas.name = "Background Canvas";
    backgroundCanvasRect = backgroundCanvas.GetComponent<RectTransform>();
    backgroundCanvasRect.pivot = new Vector2(0f, 1f);
    backgroundCanvasRect.sizeDelta = new Vector2(CANVAS_WIDTH, SPACE_BETWEEN_LINES);
    backgroundCanvasRect.transform.position += new Vector3(-CANVAS_BUFFER, CANVAS_BUFFER);

    roundTalleyParent = Instantiate(emptyObject, gameObject.transform);
    roundTalleyParent.name = "Round Talley Parent";
    roundTalleyParent.transform.position = backgroundCanvas.transform.position 
    -= new Vector3(-CANVAS_BUFFER, CANVAS_BUFFER);
    backgroundCanvas.transform.SetParent(roundTalleyParent.transform);
}

private int CalculateMoneyReward()
{
    //set combosDropped and secondsLost here from respective dependency scripts once implemented

    moneyReward = moneyRewardPot - combosDropped - secondsLost;
    return moneyReward;
}

private string GetMoneyRewardString()
{
    CalculateMoneyReward();

    if(moneyReward > 0)
    {
        return "+" + moneyReward.ToString();
    }
    else
    {
        return "0";   
    }
}

private void GiveMoneyReward()
{
    FindDependencies();

    if(moneyReward >= 0)
    {
        moneyLogic.IncreaseMoney(moneyReward);   
    }
    else
    {
        moneyLogic.DecreaseMoney(moneyReward);       
    }
}

private void ClearTalleyQueue()
{
    if(lines.Count == 0) return;
    lines.Clear();
}

public void MoveRoundTalleyUI(Vector2 position)
{
    if(roundTalleyParent == null) return;

    roundTalleyParent.transform.position = position;
}

private void DestroyLineObjects()
{
    ClearTalleyQueue();

    if(lineTextElements.Count == 0) return;

    foreach(TextElement textElement in lineTextElements)
    {
        Destroy(textElement.gameObject);
    }

    lineTextElements.Clear(); 
}

#region Money Reward Helper Methods
//modify reward amount can be called with a positive OR negative integer
public void ModifyRewardAmount(int amount)
{
    if(amount == 0) return;

    if(amount > 0)
    {
        if((moneyRewardPot + amount) <= moneyRewardPotMaximum)
        {
            moneyRewardPot += amount;                
        }
        else
        {
            moneyRewardPot = moneyRewardPotMaximum;      
        }

    }
    else if(amount < 0)
    {
        if ((moneyRewardPot + amount) >= moneyRewardPotMinimum)
        {
            moneyRewardPot += amount;   
        }
        else
        {
            moneyRewardPot = moneyRewardPotMinimum;   
        }
    }
}

public void SetRewardMaximum(int amount)
{
    if(amount <= moneyRewardPotMinimum) return;

    moneyRewardPotMaximum = amount;
}

public void SetRewardMinimum(int amount)
{
    if(amount >= moneyRewardPotMaximum) return;

    moneyRewardPotMinimum = amount;
}

#endregion

}
