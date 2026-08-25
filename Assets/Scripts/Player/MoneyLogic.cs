using UnityEngine;

public class MoneyLogic : MonoBehaviour
{
private int currentMoney = 0;
private int maxMoney = 9999;
private int minMoney = 0;
public SymbolTextElement uiElement;

void Awake()
{
    UpdateMoneyUI(currentMoney);
}

public void UpdateMoneyUI(int amount)
{
    uiElement.UpdateUIElement(amount.ToString());
}

public void IncreaseMoney(int amount)
{
    if(amount <= 0) return;

    if(currentMoney + amount < maxMoney)
    {
        currentMoney += amount;
    }
    else
    {
        currentMoney = maxMoney;    
    }

    UpdateMoneyUI(currentMoney);
}

public void DecreaseMoney(int amount)
{
    if(amount <= 0) return;

    if(currentMoney - amount >= minMoney)
    {
        currentMoney -= amount;
    }
    else
    {
        currentMoney = minMoney;
    }

    UpdateMoneyUI(currentMoney);
}
}
