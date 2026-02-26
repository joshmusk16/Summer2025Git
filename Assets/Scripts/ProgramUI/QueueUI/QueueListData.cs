using System.Collections.Generic;
using UnityEngine;

public class QueueListData : MonoBehaviour
{

private GameObject currentProgram;

public List<QueueParameter> queueList = new();

private ProgramListData attackProgramList;
private ProgramListData defenseProgramList;

private ProgramUI attackProgramUI;
private ProgramUI defenseProgramUI;

private QueueDataCollector queueDataCollector;

public GameObject player;
public PlayerLogic playerLogic;
public PlayerTargeting playerTargeting;
public CustomAnimator playerAnimator;

public GameObject dashProgramManager;

void Start()
{
    attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
    defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();
    attackProgramUI = GameObject.Find("AttackUIManager").GetComponent<ProgramUI>();
    defenseProgramUI = GameObject.Find("DefenseUIManager").GetComponent<ProgramUI>();
    queueDataCollector = FindObjectOfType<QueueDataCollector>();

    if(playerAnimator != null)
    {
        playerAnimator.OnAnimationComplete += ContinueQueue;
    }

    if(player != null)
    {
        playerLogic = player.GetComponent<PlayerLogic>();
    }
}

public Vector2 EndOfQueueDestination()
{
    if(queueList.Count == 0)
    {   
        return player.transform.position; 
    }
    else
    {
        return queueList[^1].destination;   
    }
}

public void AddProgramToQueue(ProgramType programType)
{
    GameObject nextProgramObject;

    if(programType == ProgramType.Attack)
    {
        nextProgramObject = attackProgramList.NextProgramInQueue();   
    }
    else if(programType == ProgramType.Defense)
    {
        nextProgramObject = defenseProgramList.NextProgramInQueue();
    }
    else
    {
        nextProgramObject = dashProgramManager;
    }

    if(nextProgramObject == null) return;

    if(programType == ProgramType.Attack)
    {
        attackProgramUI.UpdateQueueUIOnClick();   
    }
    else if (programType == ProgramType.Defense)
    {
        defenseProgramUI.UpdateQueueUIOnClick();      
    } 

    QueueParameter nextQueueProgram = queueDataCollector.CollectQueueData(nextProgramObject, programType);
    queueList.Add(nextQueueProgram);

    UpdateTargetingParameters(nextProgramObject, programType);

    if(queueList.Count == 1)
    {
        StartQueue(programType);
    }
}

//This method removes the designated index from the queue and every index after
void RemoveFromQueue(int startingIndex)
{
    if(startingIndex >= queueList.Count) return;

    for(int i = startingIndex; i < queueList.Count; i++)
        {
            queueList.RemoveAt(i);
        }
}

private void UpdateTargetingParameters(GameObject programObject, ProgramType programType)
{
    Program nextProgram = programObject.GetComponent<Program>();
    playerTargeting.ChangeTargetingRange(nextProgram.targetingRange, programType);
}

public void StartQueue(ProgramType programType)
{
    if(programType == ProgramType.Attack ||
    programType == ProgramType.Defense)
        {
            currentProgram = Instantiate(queueList[0].program, player.transform);
        }
    else if (programType == ProgramType.Dash)
        {
            dashProgramManager.GetComponent<Program>().FireProgram(queueList[0]);
            return;
        }

    currentProgram.GetComponent<Program>().FireProgram(queueList[0]);
}

public void ContinueQueue(ProgramType completedType)
{
    //First, remove and destroy the program that just completed
    if(queueList.Count > 0 && queueList[0].programType == completedType)
    {
        Destroy(currentProgram);
        queueList.RemoveAt(0);
    }
    else
    {
        return;
    }

    //Second, update ProgramUI and ProgramListData
    if(completedType == ProgramType.Attack)
    {
        attackProgramUI.ScrollOrSetupNewHand();
    }
    else if(completedType == ProgramType.Defense)
    {
        defenseProgramUI.ScrollOrSetupNewHand();
    }
    else if(completedType == ProgramType.Dash)
    {
        dashProgramManager.GetComponent<DashChargeFiring>().OnDashCompleted(completedType);
    }

    //Third, run the next in queue, if there is something left in the queue
    //If nothing is left in queue, return to player idle animation state
    if(queueList.Count > 0)
    {
        Debug.Log("Trying to start queue again...");
        StartQueue(queueList[0].programType);  
    }
    else
    {
        playerLogic.StartIdleAnimation(ProgramType.Other);
    }
}

void OnDestroy()
{
    playerAnimator.OnAnimationComplete -= ContinueQueue;
}

}
