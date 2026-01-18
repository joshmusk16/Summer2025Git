using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class QueueListData : MonoBehaviour
{

private GameObject currentProgram;

public List<QueueParameter> queueList = new();
public Vector2 endOfQueueDestination;

private ProgramListData attackProgramList;
private ProgramListData defenseProgramList;

private ProgramUI attackProgramUI;
private ProgramUI defenseProgramUI;

private QueueDataCollector queueDataCollector;

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
}

public void AddProgramToQueue(ProgramType programType)
{
    QueueParameter nextQueueProgram = queueDataCollector.CollectQueueData(IdentifyNextQueueProgram(programType), programType);
    queueList.Add(nextQueueProgram);
    UpdateTargetingParameters(programType);

    if(queueList.Count == 0)
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

private void UpdateTargetingParameters(ProgramType programType)
{
    Program nextProgram = IdentifyNextQueueProgram(programType).GetComponent<Program>();
    playerTargeting.ChangeTargetingRange(nextProgram.targetingRange, programType);
}

public GameObject IdentifyNextQueueProgram(ProgramType programType)
{
    if(programType == ProgramType.Attack || programType == ProgramType.Defense)
    {
        int index = 0;

        foreach(QueueParameter queueProgram in queueList)
        {
            if (programType == queueProgram.programType)
            {
                index++;
            }        
        }

        if(programType == ProgramType.Attack)
        {
            return attackProgramList.drawnPrograms[index];  
        }
        else if(programType == ProgramType.Defense)
        {
            return defenseProgramList.drawnPrograms[index];   
        }
    }
    else if(programType == ProgramType.Dash)
    {
        return dashProgramManager;
    }

    return null;
}

public void StartQueue(ProgramType programType)
{
    if(programType == ProgramType.Attack ||
    programType == ProgramType.Defense)
        {
            currentProgram = Instantiate(queueList[0].program, gameObject.transform);
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
    if(queueList.Count > 0)
    {
        StartQueue(queueList[0].programType);   
    }
}

void OnDestroy()
{
    playerAnimator.OnAnimationComplete -= ContinueQueue;
}

}
