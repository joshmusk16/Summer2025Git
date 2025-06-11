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
        currentAttackProgramAmount = attackPrograms.Count;
    }

    void Update()
    {
        currentDeckAmountDisplay.UpdateNumber(currentAttackProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalAttackProgramAmount);
        //These should not run in update, for debugging only.
    }

    public void DecreaseCurrentAtkCount()
    {
        if (currentAttackProgramAmount > 0)
        {
            currentAttackProgramAmount--;   
        }
    }
}
