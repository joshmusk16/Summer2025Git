using System.Collections.Generic;
using UnityEngine;

public class HitboxTracker : MonoBehaviour
{

private Dictionary<GameObject, List<HitBox>> hitboxGroups = new();

public void RegisterHitboxGroup(GameObject program, HitboxTiming[] hitboxTimings, int hitboxAmount)
{  
    HitBox[] temp = new HitBox[hitboxAmount];

    for(int i = 0; i < hitboxAmount; i++)
    {
        temp[i] = hitboxTimings[i].hitbox;
    }

    hitboxGroups[program] = new List<HitBox>(temp); 
}

public void UnregisterHitboxGroup(GameObject program)
{
    hitboxGroups.Remove(program);
}

public void CheckForReward(GameObject program, int rewardType)
{
    bool shouldGiveReward = false;

    switch (rewardType)
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
        GiveReward();
        UnregisterHitboxGroup(program);
    }
}

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

public void GiveReward()
{

}

}
