
using System.Collections.Generic;
using System;
using UnityEngine;

public class HitBox : CollisionBox
{

    [Header("Hitbox Properties")]
    public int damage = 1;
    public string[] hitboxTags = { "default" };

    [Header("Hitbox Behavior")]
    public bool multiHit = true;
    public float multiHitInterval = 0.1f;
    public int maxHits = -1; // -1 for unlimited

    private Dictionary<HurtBox, float> lastHitTimes = new();
    private Dictionary<HurtBox, int> hitCounts = new();

    public event Action<HurtBox, HitInfo> OnHit;

    protected override void OnDrawGizmos()
    {
        gizmoColor = Color.red;
        base.OnDrawGizmos();
    }

    public bool CanHit(HurtBox hurtbox)
    {
        if (!isActive || !hurtbox.isActive) return false;

        // Check if we can hit this hurtbox based on tags
        if (!HasMatchingTag(hurtbox)) return false;

        // Check multi-hit restrictions
        if (!multiHit && hitCounts.ContainsKey(hurtbox) && hitCounts[hurtbox] > 0)
            return false;

        // Check max hits limit
        if (maxHits > 0 && hitCounts.ContainsKey(hurtbox) && hitCounts[hurtbox] >= maxHits)
            return false;

        // Check multi-hit interval
        if (lastHitTimes.ContainsKey(hurtbox))
        {
            float timeSinceLastHit = Time.time - lastHitTimes[hurtbox];
            if (timeSinceLastHit < multiHitInterval)
                return false;
        }

        return true;
    }

    private bool HasMatchingTag(HurtBox hurtbox)
    {
        foreach (string hitboxTag in hitboxTags)
        {
            foreach (string hurtboxTag in hurtbox.hurtboxTags)
            {
                if (hitboxTag == hurtboxTag)
                    return true;
            }
        }
        return false;
    }

    public void Hit(HurtBox hurtbox)
    {
        if (!CanHit(hurtbox)) return;

        // Record hit
        lastHitTimes[hurtbox] = Time.time;
        if (!hitCounts.ContainsKey(hurtbox))
        {
            hitCounts[hurtbox] = 0;
        }
        else
        {
            hitCounts[hurtbox]++;
        }

        // Create hit info
        HitInfo hitInfo = new()
        {
            hitbox = this,
            hurtbox = hurtbox,
            damage = damage,
        };

        // Trigger events
        OnHit?.Invoke(hurtbox, hitInfo);
        Debug.Log(gameObject.name + " is hitting " + hurtbox.name + " at " + Time.time);
        hurtbox.TakeHit(hitInfo);
    }
    
    public void ResetHitRecord(HurtBox hurtbox = null)
    {
        if (hurtbox != null)
        {
            lastHitTimes.Remove(hurtbox);
            hitCounts.Remove(hurtbox);
        }
        else
        {
            lastHitTimes.Clear();
            hitCounts.Clear();
        }
    }
}


