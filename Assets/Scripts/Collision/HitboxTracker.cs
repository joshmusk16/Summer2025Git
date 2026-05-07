using System.Collections.Generic;
using UnityEngine;

public class HitboxTracker : MonoBehaviour
{

private Dictionary<GameObject, List<HitBox>> hitboxGroups = new();
public static HitboxTracker Instance { get; private set; }
public ComboBarLogic comboBar;

private void Awake()
{
    Instance = this;
    comboBar = FindObjectOfType<ComboBarLogic>();
}

public void RegisterHitboxGroup(GameObject program, HitboxTiming[] hitboxTimings, int hitboxAmount)
{  
    HitBox[] temp = new HitBox[hitboxAmount];

    for(int i = 0; i < hitboxAmount; i++)
    {
        temp[i] = hitboxTimings[i].hitbox;
    }

    hitboxGroups[program] = new List<HitBox>(temp); 
    Debug.Log("Registered Hitbox Group" + program);
}

public void UnregisterHitboxGroup(GameObject program)
{
    hitboxGroups.Remove(program);
    Debug.Log("Unregistered Hitbox Group" + program);
}

public void CheckForReward(GameObject program, int rewardRequirementType, int rewardType, int amount)
{
    if(!hitboxGroups.ContainsKey(program)) return;
    
    bool shouldGiveReward = false;

    switch (rewardRequirementType)
    {
        case 1: 
            shouldGiveReward = HasAnyHitboxHit(program);
            break;
        case 2:
            shouldGiveReward = HasEveryHitboxHit(program);
            break;
    }

    if (shouldGiveReward)
    {
        comboBar.ChangeComboBar(rewardType, amount);
        UnregisterHitboxGroup(program);
    }
}

//Reward Requirement types : 1 = HasAnyHitboxHit, 2 = HasEveryHitboxHit

private bool HasAnyHitboxHit(GameObject program)
{
    foreach(HitBox hitbox in hitboxGroups[program])
    {
        if (hitbox.hasHitOnce)
        {
            return true;    
        }
    }
    return false;
}

private bool HasEveryHitboxHit(GameObject program)
{
    foreach(HitBox hitbox in hitboxGroups[program])
    {
        if (hitbox.hasHitOnce == false)
        {
            return false;    
        }
    }
    return true;
}

}
