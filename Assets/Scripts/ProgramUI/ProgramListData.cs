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

    private GameObject currentProgram = null;

    public GameObject player;
    private ProgramInputManager programInputManager = null;

    void Start()
    {
        totalProgramAmount = programs.Count;
        UpdateCountUI();

        programInputManager = FindObjectOfType<ProgramInputManager>();
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

        handSize = Mathf.Min(handSize, drawPilePrograms.Count);

        drawnPrograms.Clear();

        for(int i = 0; i < handSize; i++)
        {
            int randomIndex = Random.Range(0, drawPilePrograms.Count);
            drawnPrograms.Add(drawPilePrograms[randomIndex]);
            drawPilePrograms.RemoveAt(randomIndex);
        }

        currentProgram = Instantiate(drawnPrograms[0], player.transform);
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
        DestroyCurrentProgram();
        drawnPrograms.RemoveAt(0);
        currentProgram = Instantiate(drawnPrograms[0], player.transform);
        
        //UpdateCountUI();
    }

    public void AddProgramsToHand(GameObject[] addPrograms, int[] indices)
    {
        if(addPrograms.Length != indices.Length) return;

        for(int i = 0; i < addPrograms.Length; i++)
        {
            drawnPrograms.Insert(indices[i], programs[i]);
        }
    }

    public void RemoveProgramsFromHand(int[] indices)
    {
        for(int i = 0; i < indices.Length; i++)
        {
            drawnPrograms.RemoveAt(indices[i]);
        }
    }

    public void MoveProgram(int startIndex, int endIndex)
    {
        if (startIndex != endIndex)
        {
            if (endIndex == 0)
            {
                DestroyCurrentProgram();
                currentProgram = Instantiate(drawnPrograms[startIndex], player.transform);
            }
            else if (startIndex == 0)
            {
                DestroyCurrentProgram();
                currentProgram = Instantiate(drawnPrograms[1], player.transform);
            }

            GameObject swap = programs[startIndex];
            drawnPrograms.RemoveAt(startIndex);
            drawnPrograms.Insert(endIndex, swap);
        }
    }

    public void DestroyCurrentProgram()
    {
        if (currentProgram != null)
        {
            Destroy(currentProgram);
            programInputManager.isAttacking = false;
            programInputManager.isDefending = false;
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
