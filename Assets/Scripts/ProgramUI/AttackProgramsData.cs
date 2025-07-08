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

    void Start()
    {
        totalAttackProgramAmount = attackPrograms.Count;

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
        if (currentAttackProgram != null)
        {
            Destroy(currentAttackProgram);   
        }

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
                Destroy(currentAttackProgram);
                currentAttackProgram = Instantiate(attackPrograms[startIndex + currentAttackProgramAmount], player.transform);
            }
            else if(startIndex == 0)
            {
                Destroy(currentAttackProgram);
                currentAttackProgram = Instantiate(attackPrograms[currentAttackProgramAmount + 1], player.transform);
            }

            endIndex += currentAttackProgramAmount;
            startIndex += currentAttackProgramAmount;

            GameObject swap = attackPrograms[startIndex];
            attackPrograms.RemoveAt(startIndex);
            attackPrograms.Insert(endIndex, swap);
        }
    }
}
