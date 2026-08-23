using System.Collections.Generic;
using UnityEngine;

public class RoundRewardUI : MonoBehaviour
{

public GameObject roundRewardUIPrefab;
private GameObject roundRewardUI;

public ButtonUIElement takeButton;
public ButtonUIElement skipButton;
public TextElement rewardCountUI;

public GameObject emptyObject;

private GameObject rewardProgramUIElement; 
private GameObject rewardProgram;
private Program programInfo;
private Vector3 REWARD_PROGRAM_SCALE = new(2f, 2f, 1f);

private int amountOfRewards = 3;

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
}

private void Update() //Update for debugging
{
    if (Input.GetKeyDown(KeyCode.F))
    {
        GenerateReward();
    }
}

    private void GenerateCanvasAndButtons()
{
    if(roundRewardUI == null)
    {
        roundRewardUI = Instantiate(roundRewardUIPrefab);

        Transform takeButtonTransform = roundRewardUI.transform.Find("TakeButton");
        Transform skipButtonTransform = roundRewardUI.transform.Find("SkipButton");
        Transform rewardCountTransform = roundRewardUI.transform.Find("RewardCountUI");

        takeButton = takeButtonTransform.gameObject.GetComponent<ButtonUIElement>();
        skipButton = skipButtonTransform.gameObject.GetComponent<ButtonUIElement>();
        rewardCountUI = rewardCountTransform.gameObject.GetComponent<TextElement>();

        takeButton.GenerateButton();
        skipButton.GenerateButton();
        UpdateRewardCountUI(amountOfRewards);

        takeButton.OnButtonPressed += GiveReward;
        skipButton.OnButtonPressed += SkipReward;
    }
}

private void GenerateReward()
{
    GenerateCanvasAndButtons(); //temporary location to call this method

    int randomRewardIndex = Random.Range(0 , uiProgramRewardPool.Count); //This should be based on a seed in the future
    rewardProgram = uiProgramRewardPool[randomRewardIndex];
    programInfo = rewardProgram.GetComponent<Program>();
    Sprite programSprite = programInfo.uiSprite;

    if(rewardProgramUIElement == null && roundRewardUI != null)
    {
        rewardProgramUIElement = Instantiate(emptyObject, Vector2.zero, Quaternion.identity, roundRewardUI.transform);
        rewardProgramUIElement.transform.localScale = REWARD_PROGRAM_SCALE;
    }
    
    rewardProgramUIElement.GetComponent<SpriteRenderer>().sprite = programSprite;
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
        UpdateRewardCountUI(amountOfRewards);
    }
    else if(amountOfRewards <= 1)
    {
        amountOfRewards--;
        UpdateRewardCountUI(amountOfRewards);

        rewardProgramUIElement.GetComponent<SpriteRenderer>().sprite = null;
        ToggleRewardButtons(false);
        //Reset scene for next round...
    }
}

public void UpdateRewardCountUI(int count)
{
    if(rewardCountUI == null) return;

    rewardCountUI.textInput = count.ToString();
    rewardCountUI.GenerateTextElement();
}

public void ToggleRewardButtons(bool enabled)
{
    if(takeButton == null || skipButton == null) return;

    takeButton.buttonIsActive = enabled;
    skipButton.buttonIsActive = enabled;
}

void OnDestroy()
{
    takeButton.OnButtonPressed -= GiveReward;
    skipButton.OnButtonPressed -= SkipReward;   
}

}
