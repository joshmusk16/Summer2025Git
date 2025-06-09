using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour
{
    public int totalAttackProgramAmount;
    public int currentAttackProgramAmount;
    public List<GameObject> attackPrograms = new();

    public NumberUI currentDeckAmountDisplay;
    public NumberUI totalDeckAmountDisplay;

    void Update()
    {
        currentDeckAmountDisplay.UpdateNumber(currentAttackProgramAmount);
        totalDeckAmountDisplay.UpdateNumber(totalAttackProgramAmount);
        //These should not run in update, for debugging only.
    }
}
