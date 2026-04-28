using UnityEngine;

public class ComboBarLogic : MonoBehaviour
{

public int currentCombo;
private const int MAX_COMBO_ALLOWED = 500;
public int startingComboAmount = 1;
public ComboBarUI comboBarUI;

void Update()
{
    if (Input.GetKeyDown(KeyCode.K))
    {
        currentCombo = Random.Range(0, 500);
        Debug.Log("Trying to display" + currentCombo);
        AddToCombo(0);
    }
}

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
    if(amount >= 0 && currentCombo + amount < MAX_COMBO_ALLOWED)
    {
        currentCombo += amount;
        comboBarUI.UpdateComboNumber(currentCombo);
    }
    else if(currentCombo + amount > MAX_COMBO_ALLOWED)
    {
        currentCombo = MAX_COMBO_ALLOWED;
        comboBarUI.UpdateComboNumber(currentCombo);
    }
}

public void RemoveFromoCombo(int amount)
{
    if(amount >= 0 && currentCombo - amount > 0)
    {
        currentCombo -= amount;
        comboBarUI.UpdateComboNumber(currentCombo);
    }
    else if(currentCombo - amount <= 0)
    {
        currentCombo = 0;
        comboBarUI.UpdateComboNumber(currentCombo);
    }
}

}

