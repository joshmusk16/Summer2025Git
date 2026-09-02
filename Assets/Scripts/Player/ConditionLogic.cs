
using UnityEngine;

public class ConditionLogic : MonoBehaviour
{

private int currentConditionAmount = 0;
private int conditionsDropped = 0;
private int highestConditionCombo = 0;
private SymbolTextElement conditionMeterUI;

private void FindDependencies()
{
    if(conditionMeterUI == null) conditionMeterUI = gameObject.GetComponent<SymbolTextElement>();    
}

private void Awake()
{
    UpdateConditionMeterUI(currentConditionAmount.ToString());
}

public void IncrementConditionMeter()
{
    currentConditionAmount++;

    if(currentConditionAmount > highestConditionCombo)
    {
        highestConditionCombo = currentConditionAmount;
    }

    UpdateConditionMeterUI(currentConditionAmount.ToString());
}

public void ResetConditionMeter(bool isFinalRoundReset)
{
    currentConditionAmount = 0;
    conditionsDropped++;

    if (isFinalRoundReset)
    {
        conditionsDropped = 0;   
        highestConditionCombo = 0;
    }

    UpdateConditionMeterUI(currentConditionAmount.ToString());
}

public int GetConditionsDropped()
{
    return conditionsDropped;
}

public int GetHighestCondition()
{
    return highestConditionCombo;
}

private void UpdateConditionMeterUI(string text)
{
    FindDependencies();
    conditionMeterUI.UpdateUIElement(text);
}

}
