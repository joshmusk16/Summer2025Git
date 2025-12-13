using System.Collections.Generic;
using UnityEngine;

public class DashChargeManager : MonoBehaviour
{

private int totalDashCharges;
private int currentDashCharges;
public List<GameObject> dashCharges = new();
public GameObject dashCharge;

private const int startingDashChargeAmount = 4;
private const int lowestAllowedDashChargeAmount = 1; 
private const int lowestSortingOrder = 1;

[SerializeField] private int totalXLength;

void Start()
{
    InitiateStartingDashCharges();
}

void InitiateStartingDashCharges()
{
    totalDashCharges = startingDashChargeAmount;
    currentDashCharges = startingDashChargeAmount;

    for(int i = 0; i < startingDashChargeAmount; i++)
    {
        GameObject newDashCharge = Instantiate(dashCharge);
        dashCharges.Add(newDashCharge);
    }
    RearrangeDashCharges();
}

public void AddDashCharge(int amount)
{
    if(currentDashCharges + amount <= totalDashCharges)
    {
        currentDashCharges += amount;    
    }
    else
    {
        currentDashCharges = totalDashCharges;    
    }

    UpdateDashChargeSprites();
}

public void RemoveDashCharge(int amount)
{
    if(currentDashCharges - amount <= 0)
    {
        currentDashCharges = 0;
    }
    else
    {
        currentDashCharges -= amount;
    }

    UpdateDashChargeSprites();
}

private void UpdateDashChargeSprites()
{
    for(int i = 0; i < dashCharges.Count; i++)
    {
        if(i < dashCharges.Count - currentDashCharges)
        {
            dashCharges[i].GetComponent<DashChargeLogic>().ChangeState(0);        
        }
        else
        {
            dashCharges[i].GetComponent<DashChargeLogic>().ChangeState(1);        
        }
    }
}

private void RearrangeDashCharges()
{
    for(int i = 0; i < dashCharges.Count; i++)
    {
        dashCharges[i].transform.position = StartingPosition() + new Vector2(ChargeSpacing() * i, 0);
        dashCharges[i].GetComponent<SpriteRenderer>().sortingOrder = lowestSortingOrder + dashCharges.Count - i;        
    }
}

private Vector2 StartingPosition()
{
    Vector2 startingPos = transform.position;
    return startingPos - new Vector2(totalXLength / 2f, 0);
}

private float ChargeSpacing()
{
    return totalDashCharges <= 1 ? 0 : totalXLength / (float)(totalDashCharges - 1);
}

public void AddTotalDashCharge(int amount)
{
    totalDashCharges += amount;
    currentDashCharges += amount;
    
    for(int i = 0; i < amount; i++)
    {
        GameObject newDashCharge = Instantiate(dashCharge);
        dashCharges.Add(newDashCharge);
    }
    
    RearrangeDashCharges();
    UpdateDashChargeSprites();
}

public void RemoveTotalDashCharge(int amount)
{
    if((totalDashCharges - amount) >= lowestAllowedDashChargeAmount)
    {
        totalDashCharges -= amount;
        currentDashCharges = Mathf.Max(0, currentDashCharges - amount);

        for(int i = 0; i < amount; i++)
        {
            int lastIndex = dashCharges.Count - 1;
            Destroy(dashCharges[lastIndex]);
            dashCharges.RemoveAt(lastIndex);
        }
        
        RearrangeDashCharges();
        UpdateDashChargeSprites();
    }
}

public bool IsDashChargeAvailable()
{
    return currentDashCharges > 0;
}
}
