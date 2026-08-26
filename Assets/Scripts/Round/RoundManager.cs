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
private RoundCountUI roundCountUI;
private RoundTransition roundTransitionAnimation;

[Header("Between Round Prefabs")]
public GameObject roundRewardObject;
public GameObject roundTalleyObject;

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
    && roundCountUI != null
    && roundTransitionAnimation != null) return;

    queueListData = FindObjectOfType<QueueListData>();
    playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
    levelManager = FindObjectOfType<LevelCollection>();
    programInputManager = FindObjectOfType<ProgramInputManager>();
    roundCountUI = FindObjectOfType<RoundCountUI>();
    roundTransitionAnimation = FindObjectOfType<RoundTransition>();
}

public void SetupNewRound()
{
    FindDependencies();

    levelManager.ResetRandomLevel();
    programInputManager.EnableInput();
    roundCountUI.IncrementRoundUI();
    QueueListData.OnProgramAddedToQueue += AfterFirstQueueEvents;
    OnAllEnemiesCleared += EndRound;
}

public void AfterFirstQueueEvents()
{
    FindDependencies();

    playerTimerLogic.StartRunningTimer();
    QueueListData.OnProgramAddedToQueue -= AfterFirstQueueEvents;
}

public void EndRound()
{
    FindDependencies();

    programInputManager.DisableInput();
    playerTimerLogic.StopRunningTimer();
    queueListData.ClearQueue();

    //Really we want AnimateRoundTransition to happen after the final animation is done playing...
    roundTransitionAnimation.AnimateRoundTransitionIn();
    
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
}
}
