using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    private List<HitBox> ActiveHitboxes = new();
    private List<HurtBox> ActiveHurtboxes = new();

    void Update()
    {
        UpdateCollisionBoxLists();
        CheckCollisions();
    }

    private void CheckCollisions()
    {
        foreach (HitBox hitbox in ActiveHitboxes)
        {
            foreach (HurtBox hurtbox in ActiveHurtboxes)
            {
                // Don't check collision with self, transform.root check the parent of each transform, 
                // so if the parent transform of the hitbox and hurtbox are the same, they won't interact with each other.
                if (hitbox.transform.root == hurtbox.transform.root)
                {
                    continue;   
                }

                if (hitbox.Overlaps(hurtbox))
                {
                    hitbox.Hit(hurtbox);
                }
            }
        }
    }

    // Manual registration methods for better performance if needed
    public void RegisterHitbox(HitBox hitbox)
    {
        if (!ActiveHitboxes.Contains(hitbox))
            ActiveHitboxes.Add(hitbox);
    }

    public void UnregisterHitbox(HitBox hitbox)
    {
        ActiveHitboxes.Remove(hitbox);
    }

    public void RegisterHurtbox(HurtBox hurtbox)
    {
        if (!ActiveHurtboxes.Contains(hurtbox))
            ActiveHurtboxes.Add(hurtbox);
    }

    public void UnregisterHurtbox(HurtBox hurtbox)
    {
        ActiveHurtboxes.Remove(hurtbox);
    }
    
    //Run this method in update, and you won't have manually add 
    // and remove every hitbox and hurtbox to ActiveHitboxes and Hurtboxes, but this is worse for performance
    private void UpdateCollisionBoxLists()
    {
        // Update hitboxes list
        ActiveHitboxes.Clear();
        HitBox[] hitboxes = FindObjectsOfType<HitBox>();
        foreach (HitBox hitbox in hitboxes)
        {
            if (hitbox.isActive)
                ActiveHitboxes.Add(hitbox);
        }

        // Update hurtboxes list
        ActiveHurtboxes.Clear();
        HurtBox[] hurtboxes = FindObjectsOfType<HurtBox>();
        foreach (HurtBox hurtbox in hurtboxes)
        {
            if (hurtbox.isActive)
                ActiveHurtboxes.Add(hurtbox);
        }
    }
}
