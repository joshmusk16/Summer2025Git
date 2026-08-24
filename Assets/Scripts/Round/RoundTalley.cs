using System.Collections.Generic;
using UnityEngine;

public class RoundTalley : MonoBehaviour
{

public GameObject backgroundCanvas; //background prefab assigned in inspector
private const float CANVAS_WIDTH = 5f;
private float FINAL_CANVAS_HEIGHT = 0;

private int moneyRewardAmount;
private int moneyRewardMaximum;
private int moneyRewardMinimum;
private int combosDropped = 0;
private int secondsLost = 0;

public GameObject emptyTextElement; //create and assign TextElement prefab
private List<string> lines = new();
private List<TextElement> lineTextElements = new();

private const float FIRST_LINE_SCALE = 1f;
private const float LOWER_LINES_SCALE = 0.75f;
private const float SPACE_BETWEEN_LINES = 0.5f;

private Vector3 offsetVector = Vector3.zero;

private bool isAnimating = false;
private float currentTime = 0;
private const float TIME_BETWEEN_LINE_GENERATION = 0.15f; //in seconds
private int lineIndex = 0;

private void Update()
{
    if (isAnimating)
    {
        currentTime += Time.deltaTime;
        if(currentTime >= TIME_BETWEEN_LINE_GENERATION)
        {
            if(lineIndex < lines.Count)
            {
                GenerateSingleLine(lines[lineIndex], offsetVector);
                currentTime = 0;
                lineIndex++;
                offsetVector -= new Vector3(0, SPACE_BETWEEN_LINES, 0);

                if(lineIndex >= lines.Count)
                {
                    offsetVector = Vector3.zero;
                    lineIndex = 0;
                    isAnimating = false;
                }
            }
        }
    }
}

private void AssignTalleyTextElements()
{
    if(isAnimating) return;

    ClearTalleyQueue();
    int reward = CalculateRewardTotal();

    lines.Add(moneyRewardAmount.ToString());
    lines.Add("-" + combosDropped.ToString() + " COMBOS DROPPED");
    lines.Add("-" + secondsLost.ToString() + " SECONDS LOST");
    
    lines.Add(reward.ToString());
}

private void StartTalleyAnimation()
{
    lineIndex = 0;
    currentTime = 0;
    isAnimating = true;  
}

private void GenerateSingleLine(string text, Vector3 position)
{
    GameObject line = Instantiate(emptyTextElement, gameObject.transform);
    TextElement textElement = line.GetComponent<TextElement>();
    lineTextElements.Add(textElement);
    
    textElement.textInput = text;
    textElement.GenerateTextElement();    
    textElement.MoveTextParent(position);
}


private int CalculateRewardTotal()
{
    //set combosDropped and secondsLost here from respective dependency scripts once implemented

    int reward = moneyRewardAmount - combosDropped - secondsLost;

    if(reward >= 0)
    {
        return reward;
    }
    else
    {
        return 0;   
    }
}

private void ClearTalleyQueue()
{
    if(lines.Count == 0) return;
    lines.Clear();
}

private void DestroyLineObjects()
{
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
        if((moneyRewardAmount + amount) <= moneyRewardMaximum)
        {
            moneyRewardAmount += amount;                
        }
        else
        {
            moneyRewardAmount = moneyRewardMaximum;      
        }

    }
    else if(amount < 0)
    {
        if ((moneyRewardAmount + amount) >= moneyRewardMinimum)
        {
            moneyRewardAmount += amount;   
        }
        else
        {
            moneyRewardAmount = moneyRewardMinimum;   
        }
    }
}

public void SetRewardMaximum(int amount)
{
    if(amount <= moneyRewardMinimum) return;

    moneyRewardMaximum = amount;
}

public void SetRewardMinimum(int amount)
{
    if(amount >= moneyRewardMaximum) return;

    moneyRewardMinimum = amount;
}

#endregion

}
