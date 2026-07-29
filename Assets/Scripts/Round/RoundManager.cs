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

    void Awake()
    {
        SetupNewRound();
    }

    private void FindDependencies()
    {
        queueListData = FindObjectOfType<QueueListData>();
        playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
        levelManager = FindObjectOfType<LevelCollection>();
        programInputManager = FindObjectOfType<ProgramInputManager>();
    }

    public void SetupNewRound()
    {
        if(queueListData == null 
        || playerTimerLogic == null
        || levelManager == null
        || programInputManager == null) FindDependencies();

        levelManager.ResetRandomLevel();
        programInputManager.EnableInput();
        QueueListData.OnProgramAddedToQueue += AfterFirstQueueEvents;
        OnAllEnemiesCleared += EndRound;
    }

    public void AfterFirstQueueEvents()
    {
        if(queueListData == null 
        || playerTimerLogic == null
        || levelManager == null
        || programInputManager == null) FindDependencies();

        playerTimerLogic.StartRunningTimer();
        QueueListData.OnProgramAddedToQueue -= AfterFirstQueueEvents;
    }

    public void EndRound()
    {
        if(queueListData == null 
        || playerTimerLogic == null
        || levelManager == null
        || programInputManager == null) FindDependencies();

        programInputManager.DisableInput();
        playerTimerLogic.StopRunningTimer();
        queueListData.ClearQueue();
        
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
