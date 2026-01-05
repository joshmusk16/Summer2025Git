using UnityEngine;

[System.Serializable]
public struct QueueParameter
{
    public GameObject program;         //program prefab object
    public ProgramType programType;    //attack, defense, or dash program
    public Vector2 destination;        //movement destination for movement programs
    public Sprite previewSprite;       //rendered sprite on the board after queue
    public int facedDirection;         //0 for left, 1 for right
}

public class QueueDataCollector : MonoBehaviour
{
    private MouseTracker mouseTracker;
    private QueueListData queueListData;
    private PlayerTargeting playerTargeting;
    
    void Start()
    {
        mouseTracker = FindObjectOfType<MouseTracker>();
        queueListData = FindObjectOfType<QueueListData>();
        playerTargeting = FindObjectOfType<PlayerTargeting>();
    }

    public void IdentifyNextProgramToQueue(ProgramType programType)
    {  
        GameObject program = queueListData.IdentifyNextQueueProgram(programType);
        CollectQueueData(program, programType);   //Vector.zero is a placeholder
    }

    //newDestination needs to be passed from the PlayerTargeting script which still needs to be modified to account for this
    public void CollectQueueData(GameObject program, ProgramType programType)
    {
        Program programData = program.GetComponent<Program>();

        Vector2 currentMousePos = mouseTracker.worldPosition;
        QueueParameter queueParameter;

        queueParameter.program = program;
        queueParameter.programType = programData.programType;
        queueParameter.previewSprite = programData.animSprites[0];

        if (programData.isMovementProgram)
        {
            queueParameter.destination = playerTargeting.ProgressTargetingOrigin();
        }
        else
        {
            queueParameter.destination = queueListData.endOfQueueDestination; 
        }
        
        if(currentMousePos.x > queueParameter.destination.x)
        {
            queueParameter.facedDirection = 1;
        }
        else
        {
            queueParameter.facedDirection = 0;
        }

        queueListData.AddToQueue(queueParameter);
    }
}
