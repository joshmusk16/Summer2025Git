using System.Collections.Generic;
using UnityEngine;

public class ProgramListData : MonoBehaviour
{
    public int totalProgramAmount;

    public List<GameObject> programs = new();
    public List<GameObject> drawPilePrograms = new();
    public List<GameObject> drawnPrograms = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    public GameObject player;
    public PlayerTargeting playerTargeting;

    private int nextInQueueIndex = 0;

    void Start()
    {
        totalProgramAmount = programs.Count;
        UpdateCountUI();
    }

    public GameObject NextProgramInQueue()
    {
        if(drawnPrograms != null && nextInQueueIndex < drawnPrograms.Count)
        {
            return drawnPrograms[nextInQueueIndex++];        
        }

        return null;
    }

    public int DetermineHandSize(int handSize)
    {
        ResetDrawPile();
        return Mathf.Min(handSize, drawPilePrograms.Count);
    }

    public void DrawNewHand(int handSize)
    {
        if(drawPilePrograms == null || drawPilePrograms.Count == 0) return;

        if(handSize <= 0) return;

        nextInQueueIndex = 0;

        handSize = Mathf.Min(handSize, drawPilePrograms.Count);

        drawnPrograms.Clear();

        for(int i = 0; i < handSize; i++)
        {
            int randomIndex = Random.Range(0, drawPilePrograms.Count);
            drawnPrograms.Add(drawPilePrograms[randomIndex]);
            drawPilePrograms.RemoveAt(randomIndex);
        }

        if(playerTargeting != null)
        {
            Program firstProgram = drawnPrograms[0].GetComponent<Program>();
            playerTargeting.InitializeTargetingValue(firstProgram);
        }

        UpdateCountUI();
    }

    public void ResetDrawPile()
    {
        if(drawPilePrograms == null || drawPilePrograms.Count == 0)
        {
            drawPilePrograms = new List<GameObject>(programs);
        }
    }

    public void ScrollCurrentProgram()
    {
        drawnPrograms.RemoveAt(0);
        
        nextInQueueIndex--;
        UpdateCountUI();
    }

    public void AddProgramsToHand(GameObject[] addPrograms, int[] indices)
    {
        if(addPrograms.Length != indices.Length) return;
        
        foreach(int index in indices)
        {
            if(index == 0)
            {
                return;
            }
        }

        for(int i = 0; i < addPrograms.Length; i++)
        {
            drawnPrograms.Insert(indices[i], addPrograms[i]);
        }
    }

    public void RemoveProgramsFromHand(int[] indices)
    {
        foreach(int index in indices)
        {
            if(index <= 0 || index >= drawnPrograms.Count)
            {
                return;
            }
        }

        for(int i = 0; i < indices.Length; i++)
        {
            drawnPrograms.RemoveAt(indices[i]);
        }
    }

    public void MoveProgram(int startIndex, int endIndex)
    {
        if (startIndex != endIndex && startIndex < drawnPrograms.Count && endIndex < drawnPrograms.Count)
        {
            GameObject movedProgram = drawnPrograms[startIndex];
            drawnPrograms.RemoveAt(startIndex);
            drawnPrograms.Insert(endIndex, movedProgram);
        }
    }

    public void UpdateCountUI()
    {
        currentDeckAmountDisplay.UpdateNumber(drawPilePrograms.Count);
        totalDeckAmountDisplay.UpdateNumber(totalProgramAmount);
    }

    public bool AreProgramsAvailable()
    {
        if (drawnPrograms.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
