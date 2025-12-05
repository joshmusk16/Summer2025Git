using System.Collections.Generic;
using UnityEngine;

public class DashChargeManager : MonoBehaviour
{

public int totalDashCharges;
public int currentDashCharges;
public List<GameObject> dashCharges = new();
public GameObject dashCharge;

private const int startingDashChargeAmount = 3;
private const int lowestAllowedDashChargeAmount = 1; 

[SerializeField] private int totalXLength;
[SerializeField] private int lowestSortingOrder;
[SerializeField] private Sprite chargedSprite;
[SerializeField] private Sprite unchargedSprite;

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
        if(i < currentDashCharges)
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

//Fix this to add more than one dash charge and take int amount into account
public void AddTotalDashCharge(int amount)
{
    totalDashCharges += amount;
    GameObject newDashCharge = Instantiate(dashCharge); //add transform parameter?
    dashCharges.Add(newDashCharge);
    RearrangeDashCharges();
}

public void RemoveTotalDashCharge(int amount)
{
    if((totalDashCharges - amount) >= lowestAllowedDashChargeAmount)
    {
        totalDashCharges -= amount;
        for(int i = 0; i < amount; i++)
            {
                Destroy(dashCharges[0]);
                dashCharges.RemoveAt(0);
            }
        RearrangeDashCharges();
    }
}
}
