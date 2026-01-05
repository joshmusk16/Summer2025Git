using System.Collections.Generic;
using UnityEngine;

public class QueueListData : MonoBehaviour
{

public List<QueueParameter> queueList = new();
public Vector2 endOfQueueDestination;

private ProgramListData attackProgramList;
private ProgramListData defenseProgramList;

void Start()
{
    attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
    defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();
}

//Adds one program to the queue
public void AddToQueue(QueueParameter newProgram)
{
    queueList.Add(newProgram);
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
    else
    {
        //How will dash program case be handled / returned?
    }

    return null;
}

public void StartQueue()
{
        
}

public void ContinueQueue()
{
        
}

}
