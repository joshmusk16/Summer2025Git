using System.Collections.Generic;
using UnityEngine;

public class RoundRewardUI : MonoBehaviour
{

public ButtonUIElement takeButton;
public ButtonUIElement skipButton;
public GameObject emptyObject; //prefab emptyObject to set spriterenderer

public GameObject rewardProgramUI; 
public GameObject rewardProgramUIBackground;

private GameObject rewardProgram;
private Program programInfo;

private int amountOfRewards = 0;

private ProgramListData attackProgramData;
private ProgramListData defenseProgramData;

public List<GameObject> uiProgramRewardPool = new(); //all the prefab Program gameObjects

void Awake()
{
    FindDependencies();   
}

private void FindDependencies()
{
    if(attackProgramData != null &&
    defenseProgramData != null) return;

    attackProgramData = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
    defenseProgramData = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();

    if(takeButton != null)
    {
        takeButton.OnButtonPressed += GiveReward;
    }

    if(skipButton != null)
    {
        skipButton.OnButtonPressed += SkipReward;
    }
}

private void GenerateReward()
{
    int randomRewardIndex = Random.Range(0 , uiProgramRewardPool.Count - 1);   //This should be based on a seed in the future
    rewardProgram = uiProgramRewardPool[randomRewardIndex];
    programInfo = rewardProgram.GetComponent<Program>();
    Sprite programSprite = programInfo.uiSprite;

    if(rewardProgramUI == null)
    {
        rewardProgramUI = Instantiate(emptyObject); //add appropriate parent and position later    
    }
    
    rewardProgramUI.GetComponent<SpriteRenderer>().sprite = programSprite;
}

private void GiveReward()
{
    if(programInfo == null) return;

    if(programInfo.programType == ProgramType.Attack)
    {
        attackProgramData.AddProgramToDeck(rewardProgram);
    }
    else if (programInfo.programType == ProgramType.Defense)
    {
        defenseProgramData.AddProgramToDeck(rewardProgram);
    }

    DecreaseRewardCounter();
}

private void SkipReward()
{


    DecreaseRewardCounter();
}

private void DecreaseRewardCounter()
{
    if(amountOfRewards > 1)
    {
        GenerateReward();
        amountOfRewards--;
        //Update reward count ui here once its implemented
    }
    else if(amountOfRewards <= 1)
    {
        amountOfRewards--;
        //Update reward count ui here once its implemented

        rewardProgramUI.GetComponent<SpriteRenderer>().sprite = null;
        ToggleRewardButtons(false);
        //Reset scene for next round...
    }
}

public void ToggleRewardButtons(bool enabled)
{
    if(takeButton == null || skipButton == null) return;

    takeButton.buttonIsActive = enabled;
    skipButton.buttonIsActive = enabled;
}


}
