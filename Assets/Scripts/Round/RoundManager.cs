using UnityEngine;
using System;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{

[Header("Enemy Tracking")]
[SerializeField] public static List<GameObject> enemies = new();
public static event Action OnAllEnemiesCleared;

[Header("Dependencies")]
private QueueListData queueListData;
private PlayerTimerLogic playerTimerLogic;
private LevelCollection levelManager;
private ProgramInputManager programInputManager;

private TimeManager timeManager;
private ComboBarLogic comboBarLogic;

private RoundCountUI roundCountUI;
private RoundTransition roundTransitionAnimation;
private RoundRewardUI roundRewardUI;
private RoundTalley roundTalleyUI;

[Header("RoundUI Spawn Positions")]
private const float ROUND_REWARD_YPOSITION = 3;
private Vector2 roundRewardSpawnPosition = new(2.5f, ROUND_REWARD_YPOSITION);
private Vector2 roundTalleySpawnPosition = new(-10, ROUND_REWARD_YPOSITION);
private bool allRewardsAreTaken = false;
private bool talleyIsFinished = false;

// private void Update() //Update for debugging
// {
//     if (Input.GetKeyDown(KeyCode.F))
//     {
//         AfterRoundTransitionIn();
//     }
// }

void Awake()
{
    SetupNewRound();
}

private void FindDependencies()
{
    if(queueListData != null 
    && playerTimerLogic != null
    && levelManager != null
    && programInputManager != null
    && timeManager != null
    && comboBarLogic != null
    && roundCountUI != null
    && roundTransitionAnimation != null
    && roundRewardUI != null
    && roundTalleyUI != null) return;

    queueListData = FindObjectOfType<QueueListData>();
    playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
    levelManager = FindObjectOfType<LevelCollection>();
    programInputManager = FindObjectOfType<ProgramInputManager>();
    timeManager = FindObjectOfType<TimeManager>();
    comboBarLogic = FindObjectOfType<ComboBarLogic>();
    roundCountUI = FindObjectOfType<RoundCountUI>();
    roundTransitionAnimation = FindObjectOfType<RoundTransition>();
    roundRewardUI = gameObject.GetComponent<RoundRewardUI>();
    roundTalleyUI = gameObject.GetComponent<RoundTalley>();
}

public void SetupNewRound()
{
    FindDependencies();

    roundTalleyUI.ResetRoundTalleyUI();
    roundRewardUI.ResetRewardUI();

    //timeManager.ResetGameSpeed();
    comboBarLogic.ResetComboBar();

    levelManager.ResetRandomLevel();
    programInputManager.EnableInput();
    roundCountUI.IncrementRoundUI();
    playerTimerLogic.RecordTimeAtRoundStart();

    QueueListData.OnProgramAddedToQueue += AfterFirstQueueEvents;
    OnAllEnemiesCleared += EndRound;
}

public void AfterFirstQueueEvents()
{
    FindDependencies();

    playerTimerLogic.StartRunningTimer();
    QueueListData.OnProgramAddedToQueue -= AfterFirstQueueEvents;
}

public void AfterRoundTransitionIn()
{
    FindDependencies();

    roundTalleyUI.StartTalleyEvent(roundTalleySpawnPosition);
    roundRewardUI.StartRewardEvent(roundRewardSpawnPosition);

    roundTransitionAnimation.OnAnimationInFinish -= AfterRoundTransitionIn;
}

public void CheckForRoundEventFinished(bool state, int stateToUpdate)
{
    if(stateToUpdate == 0)
    {
        allRewardsAreTaken = state; 
    }
    else if (stateToUpdate == 1)
    {
        talleyIsFinished = state;  
    }

    if(allRewardsAreTaken && talleyIsFinished)
    {
        SetupNewRound();
        roundTransitionAnimation.AnimateRoundTransitionOut();
        allRewardsAreTaken = false;
        talleyIsFinished = false;
    }
}

public void EndRound()
{
    FindDependencies();

    programInputManager.DisableInput();
    playerTimerLogic.StopRunningTimer();
    queueListData.ClearQueue();

    //Really we want AnimateRoundTransition to happen after the final player/program animation is done playing...
    roundTransitionAnimation.AnimateRoundTransitionIn();
    roundTransitionAnimation.OnAnimationInFinish += AfterRoundTransitionIn;
    
    //SetupNewRound();
}

#region Enemy Tracker Methods

public static void RegisterEnemy(GameObject enemy)
{
    if(enemies.Contains(enemy)) return;
    enemies.Add(enemy);
}

public static void UnregisterEnemy(GameObject enemy)
{
    if(enemies.Contains(enemy) == false) return;
    enemies.Remove(enemy);

    if(enemies.Count == 0)
    {
        OnAllEnemiesCleared?.Invoke();
    }
}

public int CheckEnemiesRemaining()
{
    return enemies.Count;
}

#endregion

void OnDestroy()
{
    QueueListData.OnProgramAddedToQueue -= AfterFirstQueueEvents;
    OnAllEnemiesCleared -= EndRound;
    roundTransitionAnimation.OnAnimationInFinish -= AfterRoundTransitionIn;
}
}
