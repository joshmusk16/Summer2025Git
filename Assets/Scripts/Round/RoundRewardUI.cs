using System.Collections.Generic;
using UnityEngine;

public class RoundRewardUI : MonoBehaviour
{

public GameObject roundRewardUIPrefab;
private GameObject roundRewardUI;

private ButtonUIElement takeButton;
private ButtonUIElement skipButton;
private TextElement rewardCountUI;
private GameObject rewardCanvas;

public GameObject emptyObject;

private GameObject rewardProgramUIElement; 
private GameObject rewardProgram;
private Program programInfo;
private Vector3 REWARD_PROGRAM_SCALE = new(2f, 2f, 1f);

private const int REWARDS_DEBUG_AMOUNT = 3;
private int amountOfRewards = REWARDS_DEBUG_AMOUNT;

private ProgramListData attackProgramData;
private ProgramListData defenseProgramData;
private RoundManager roundManager;

public List<GameObject> uiProgramRewardPool = new(); //all the prefab Program gameObjects

void Awake()
{
    FindDependencies();   
}

private void FindDependencies()
{
    if(attackProgramData != null &&
    defenseProgramData != null
    && roundManager != null) return;

    attackProgramData = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
    defenseProgramData = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();
    roundManager = FindObjectOfType<RoundManager>();
}

private void Update() //Update for debugging
{
    if (Input.GetKeyDown(KeyCode.F))
    {
        //GenerateReward();
    }
}

    private void GenerateCanvasAndButtons(Vector2 parentPosition)
{
    if(roundRewardUI == null)
    {
        roundRewardUI = Instantiate(roundRewardUIPrefab, parentPosition, Quaternion.identity, gameObject.transform);
        roundRewardUI.name = "RoundRewardUI";

        Transform takeButtonTransform = roundRewardUI.transform.Find("TakeButton");
        Transform skipButtonTransform = roundRewardUI.transform.Find("SkipButton");
        Transform rewardCountTransform = roundRewardUI.transform.Find("RewardCountUI");
        Transform rewardCanvasTransform = roundRewardUI.transform.Find("RewardCanvas");

        takeButton = takeButtonTransform.gameObject.GetComponent<ButtonUIElement>();
        skipButton = skipButtonTransform.gameObject.GetComponent<ButtonUIElement>();
        rewardCountUI = rewardCountTransform.gameObject.GetComponent<TextElement>();
        rewardCanvas = rewardCanvasTransform.gameObject;

        takeButton.GenerateButton();
        skipButton.GenerateButton();
        UpdateRewardCountUI(amountOfRewards); //might need to move

        takeButton.OnButtonPressed += GiveReward;
        skipButton.OnButtonPressed += SkipReward;
    }
}

public void StartRewardEvent(Vector2 parentPosition)
{
    GenerateCanvasAndButtons(parentPosition);
    GenerateReward();
}

private void GenerateReward()
{
    int randomRewardIndex = Random.Range(0 , uiProgramRewardPool.Count); //This should be based on a seed in the future
    rewardProgram = uiProgramRewardPool[randomRewardIndex];
    programInfo = rewardProgram.GetComponent<Program>();
    Sprite programSprite = programInfo.uiSprite;

    if(rewardProgramUIElement == null && roundRewardUI != null)
    {
        rewardProgramUIElement = Instantiate(emptyObject, rewardCanvas.transform);
        rewardProgramUIElement.name = "Reward Program";
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

        roundManager.CheckForRoundEventFinished(true, 0);
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

public void MoveRoundRewardUI(Vector2 position)
{
    if(roundRewardUI == null) return;

    roundRewardUI.transform.position = position;
}

public void ResetRewardUI()
{
    if(roundRewardUI != null) Destroy(roundRewardUI);
    amountOfRewards = REWARDS_DEBUG_AMOUNT;
}

private void OnDestroy()
{
    if(takeButton != null) takeButton.OnButtonPressed -= GiveReward;
    if(skipButton != null) skipButton.OnButtonPressed -= SkipReward;   
}

}
