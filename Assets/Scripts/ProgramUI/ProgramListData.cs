using System.Collections.Generic;
using UnityEngine;

public class ProgramListData : MonoBehaviour
{
    public int totalProgramAmount;
    public int currentProgramAmount = 0;
    public List<GameObject> programs = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    private GameObject currentProgram = null;

    private GameObject player = null;
    private ProgramInputManager programInputManager = null;

    void Start()
    {
        totalProgramAmount = programs.Count;

        programInputManager = FindObjectOfType<ProgramInputManager>();
        PlayerLogic playerLogic = FindObjectOfType<PlayerLogic>();

        if (playerLogic != null)
        {
            player = playerLogic.gameObject;
        }

        currentProgram = Instantiate(programs[currentProgramAmount], player.transform);
    }

    public void IncreaseCurrentCount()
    {
        DestroyCurrentProgram();

        if (currentProgramAmount < totalProgramAmount)
        {
            currentProgramAmount++;

            if (currentProgramAmount != totalProgramAmount)
            {
                currentProgram = Instantiate(programs[currentProgramAmount], player.transform);
            }
        }
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

            UpdateCountUI();
        }
    }

    public void DestroyCurrentProgram()
    {
        if (currentProgram != null)
        {
            Destroy(currentProgram);
            programInputManager.isAttacking = false;
        }
    }

    public void UpdateCountUI()
    {
        currentDeckAmountDisplay.UpdateNumber(totalProgramAmount - currentProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalProgramAmount); 
    }
}
