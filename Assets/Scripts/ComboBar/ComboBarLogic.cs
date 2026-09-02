using UnityEngine;

public class ComboBarLogic : MonoBehaviour
{

public static int currentCombo;
private const int MAX_COMBO_ALLOWED = 500;
public const int STARTING_COMBO_AMOUNT = 1;
public ComboBarUI comboBarUI;

void Start()
{
    ResetComboBar();
}

public void ResetComboBar()
{
    if(STARTING_COMBO_AMOUNT > 0 && STARTING_COMBO_AMOUNT < MAX_COMBO_ALLOWED)
    {
        currentCombo = STARTING_COMBO_AMOUNT;
        comboBarUI.UpdateComboNumberUI(currentCombo);   
    }
}

private void AddToCombo(int amount)
{
    if(amount > 0 && currentCombo + amount < MAX_COMBO_ALLOWED)
    {
        currentCombo += amount;
    }
    else if(currentCombo + amount > MAX_COMBO_ALLOWED)
    {
        currentCombo = MAX_COMBO_ALLOWED;
    }

    comboBarUI.UpdateComboNumberUI(currentCombo); 
}

private void RemoveFromCombo(int amount)
{
    if(amount >= 0 && currentCombo - amount > 0)
    {
        currentCombo -= amount;
    }
    else if(currentCombo - amount <= 0)
    {
        currentCombo = 0;
    }

    comboBarUI.UpdateComboNumberUI(currentCombo); 
}

private void MultiplyCombo(float amount)
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

    comboBarUI.UpdateComboNumberUI(currentCombo); 
}

private void DivideCombo(int amount)
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
    
    comboBarUI.UpdateComboNumberUI(currentCombo); 
}

//Types: Adding to combo = 1, Removing from combo = 2, multiply combo = 3, divide combo = 4.

public void ChangeComboBar(int type, float amount)
{
    if(type < 0 || type > 4) return;

    switch (type)
    {
        case 0: 
            break;
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

