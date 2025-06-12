using System.Collections.Generic;
using UnityEngine;

public class AttackProgramsData : MonoBehaviour
{
    public int totalAttackProgramAmount;
    public int currentAttackProgramAmount;
    public List<GameObject> attackPrograms = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    void Start()
    {
        totalAttackProgramAmount = attackPrograms.Count;
        currentAttackProgramAmount = 0;
    }

    void Update()
    {
        currentDeckAmountDisplay.UpdateNumber(currentAttackProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalAttackProgramAmount);
        //These should not run in update, for debugging only.
    }

    public void IncreaseCurrentAtkCount()
    {
        if (currentAttackProgramAmount < totalAttackProgramAmount)
        {
            currentAttackProgramAmount++;
        }
    }

    public void MoveAttackProgram(int startIndex, int endIndex)
    {
        endIndex += currentAttackProgramAmount;
        startIndex += currentAttackProgramAmount;

        GameObject swap = attackPrograms[startIndex];
        attackPrograms.RemoveAt(startIndex);
        attackPrograms.Insert(endIndex, swap);
    }
}
