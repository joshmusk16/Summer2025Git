using UnityEngine;

public class ComboBarLogic : MonoBehaviour
{

public static int currentCombo;
private const int MAX_COMBO_ALLOWED = 500;
public int startingComboAmount = 1;
public ComboBarUI comboBarUI;

void Start()
{
    InitializeComboBar();
}

public void InitializeComboBar()
{
    if(startingComboAmount > 0 && startingComboAmount < MAX_COMBO_ALLOWED)
    {
        currentCombo = startingComboAmount;
        comboBarUI.UpdateComboNumber(currentCombo);   
    }
}

public void AddToCombo(int amount)
{
    if(amount > 0 && currentCombo + amount < MAX_COMBO_ALLOWED)
    {
        currentCombo += amount;
    }
    else if(currentCombo + amount > MAX_COMBO_ALLOWED)
    {
        currentCombo = MAX_COMBO_ALLOWED;
    }

    comboBarUI.UpdateComboNumber(currentCombo); 
}

public void RemoveFromCombo(int amount)
{
    if(amount >= 0 && currentCombo - amount > 0)
    {
        currentCombo -= amount;
    }
    else if(currentCombo - amount <= 0)
    {
        currentCombo = 0;
    }

    comboBarUI.UpdateComboNumber(currentCombo); 
}

public void MultiplyCombo(float amount)
{
    int newComboAmount = Mathf.FloorToInt(currentCombo * amount);

    if(amount > 1 && newComboAmount < MAX_COMBO_ALLOWED)
    {
        currentCombo = newComboAmount;
    }
    else if(newComboAmount > MAX_COMBO_ALLOWED)
    {
        currentCombo = MAX_COMBO_ALLOWED;
    }

    comboBarUI.UpdateComboNumber(currentCombo); 
}

public void DivideCombo(int amount)
{
    int newComboAmount = Mathf.FloorToInt(currentCombo / amount);

    if(amount > 1 && newComboAmount < MAX_COMBO_ALLOWED)
    {
        currentCombo = newComboAmount;
    }
    else if(newComboAmount > MAX_COMBO_ALLOWED)
    {
        currentCombo = MAX_COMBO_ALLOWED;
    }
    
    comboBarUI.UpdateComboNumber(currentCombo); 
}

//Types: Adding to combo = 1, Removing from combo = 2, multiply combo = 3, divide combo = 4.

public void ChangeComboBar(int type, float amount)
{
    if(type < 1 || type > 4) return;

    switch (type)
    {
        case 1: 
            AddToCombo((int)amount);
            break;
        case 2:
            RemoveFromCombo((int)amount);
            break;
        case 3:
            MultiplyCombo(amount);
            break;  
        case 4:
            DivideCombo((int)amount);
            break;     
    }
}


}

