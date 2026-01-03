using System.Collections.Generic;
using UnityEngine;

public class QueueListData : MonoBehaviour
{

public List<QueueParameter> queueList = new();
public int amountInQueue = 0;
public Vector2 endOfQueueDestination;

//Adds one program to the queue
public void AddToQueue(QueueParameter newProgram)
{
    amountInQueue++;
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

    amountInQueue -= queueList.Count - startingIndex;
}

}
