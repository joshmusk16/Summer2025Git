using System.Collections.Generic;
using UnityEngine;

public class ProgramListNew : MonoBehaviour
{
    public int totalProgramAmount;
    public int currentProgramAmount = 0;

    public List<GameObject> programs = new();
    public List<GameObject> drawPilePrograms = new();
    public List<GameObject> drawnPrograms = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    private GameObject currentProgram = null;

    private GameObject player = null;
    private ProgramInputManager programInputManager = null;

    void Start()
    {
        totalProgramAmount = programs.Count;
        UpdateCountUI();

        programInputManager = FindObjectOfType<ProgramInputManager>();
        PlayerLogic playerLogic = FindObjectOfType<PlayerLogic>();

        if (playerLogic != null)
        {
            player = playerLogic.gameObject;
        }

        //currentProgram = Instantiate(programs[currentProgramAmount], player.transform);
    }

    public int DetermineHandSize(int handSize)
    {
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
            int randomIndex = Random.Range(i, drawPilePrograms.Count);
            drawnPrograms.Add(drawPilePrograms[randomIndex]);
            drawPilePrograms.RemoveAt(randomIndex);
        }

        currentProgram = Instantiate(drawnPrograms[0], player.transform);
    }

    public void ResetDrawPile()
    {
        drawPilePrograms = new List<GameObject>(programs);
    }

    public void ScrollCurrentProgram()
    {
        DestroyCurrentProgram();
        drawnPrograms.RemoveAt(0);
        currentProgram = Instantiate(drawnPrograms[0], player.transform);
        
        //UpdateCountUI();
    }

    public void MoveProgram(int startIndex, int endIndex)
    {
        if (startIndex != endIndex)
        {
            if (endIndex == 0)
            {
                DestroyCurrentProgram();
                currentProgram = Instantiate(programs[startIndex + currentProgramAmount], player.transform);
            }
            else if (startIndex == 0)
            {
                DestroyCurrentProgram();
                currentProgram = Instantiate(programs[currentProgramAmount + 1], player.transform);
            }

            endIndex += currentProgramAmount;
            startIndex += currentProgramAmount;

            GameObject swap = programs[startIndex];
            programs.RemoveAt(startIndex);
            programs.Insert(endIndex, swap);
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
        currentDeckAmountDisplay.UpdateNumber(totalProgramAmount - currentProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalProgramAmount);
    }

    public bool AreProgramsAvailable()
    {
        if (currentProgramAmount < totalProgramAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
