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
    private QueueListData queueManager;
    
    void Start()
    {
        mouseTracker = FindObjectOfType<MouseTracker>();
        queueManager = FindObjectOfType<QueueListData>();
    }

    public QueueParameter CollectQueueData(GameObject program, Vector2 newDestination)
    {
        Program programData = program.GetComponent<Program>();

        Vector2 currentEndofQueuePos = queueManager.endOfQueueDestination;
        Vector2 currentMousePos = mouseTracker.worldPosition;
        QueueParameter queueParameter;

        queueParameter.program = program;
        queueParameter.programType = programData.programType;
        queueParameter.previewSprite = programData.animSprites[0];

        if (programData.isMovementProgram)
        {
            queueParameter.destination = newDestination;
        }
        else
        {
            queueParameter.destination = currentEndofQueuePos; 
        }
        
        if(currentMousePos.x > queueParameter.destination.x)
        {
            queueParameter.facedDirection = 1;
        }
        else
        {
            queueParameter.facedDirection = 0;
        }

        queueManager.AddToQueue(queueParameter);
        return queueParameter;
    }

}
