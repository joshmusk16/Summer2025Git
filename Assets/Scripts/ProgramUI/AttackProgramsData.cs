using System.Collections.Generic;
using UnityEngine;

public class AttackProgramsData : MonoBehaviour
{
    public int totalAttackProgramAmount;
    public int currentAttackProgramAmount = 0;
    public List<GameObject> attackPrograms = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    private GameObject currentAttackProgram = null;

    private GameObject player = null;
    private ProgramInputManager programInputManager = null;

    void Start()
    {
        totalAttackProgramAmount = attackPrograms.Count;

        programInputManager = FindObjectOfType<ProgramInputManager>();
        PlayerLogic playerLogic = FindObjectOfType<PlayerLogic>();

        if (playerLogic != null)
        {
            player = playerLogic.gameObject;
        }

        currentAttackProgram = Instantiate(attackPrograms[currentAttackProgramAmount], player.transform);
    }

    void Update()
    {
        currentDeckAmountDisplay.UpdateNumber(totalAttackProgramAmount - currentAttackProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalAttackProgramAmount);
        //These should not run in update, for debugging only.
    }

    public void IncreaseCurrentAtkCount()
    {
        DestroyCurrentAttackProgram();

        if (currentAttackProgramAmount < totalAttackProgramAmount)
        {
            currentAttackProgramAmount++;

            if (currentAttackProgramAmount != totalAttackProgramAmount)
            {
                currentAttackProgram = Instantiate(attackPrograms[currentAttackProgramAmount], player.transform);
            }
        }
    }

    public void MoveAttackProgram(int startIndex, int endIndex)
    {
        if (startIndex != endIndex)
        {
            if (endIndex == 0)
            {
                DestroyCurrentAttackProgram();
                currentAttackProgram = Instantiate(attackPrograms[startIndex + currentAttackProgramAmount], player.transform);
            }
            else if (startIndex == 0)
            {
                DestroyCurrentAttackProgram();
                currentAttackProgram = Instantiate(attackPrograms[currentAttackProgramAmount + 1], player.transform);
            }

            endIndex += currentAttackProgramAmount;
            startIndex += currentAttackProgramAmount;

            GameObject swap = attackPrograms[startIndex];
            attackPrograms.RemoveAt(startIndex);
            attackPrograms.Insert(endIndex, swap);
        }
    }

    public void DestroyCurrentAttackProgram()
    {
        if (currentAttackProgram != null)
        {
            Destroy(currentAttackProgram);
            programInputManager.isAttacking = false;   
        }
    }
}
