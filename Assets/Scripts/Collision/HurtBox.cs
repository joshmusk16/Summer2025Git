using System;
using UnityEngine;

public class HurtBox : CollisionBox
{
    [Header("Hurtbox Properties")]
    public string[] hurtboxTags = { "default" };
    public bool invulnerable = false;
    public float invulnerabilityDuration = 0f;

    public event Action<HitInfo> OnHit;
    public event Action OnInvulnerabilityStart;
    public event Action OnInvulnerabilityEnd;

    protected override void OnDrawGizmos()
    {
        gizmoColor = invulnerable ? Color.yellow : Color.blue;
        base.OnDrawGizmos();
    }
    void Update()
    {
        // Handle invulnerability timing
        if (invulnerable && invulnerabilityDuration > 0f)
        {
            invulnerabilityDuration -= Time.deltaTime;
        }
        else
        {
            invulnerable = false;
        }
    }
    
    public void TakeHit(HitInfo hitInfo)
    {
        if (invulnerable) return;
        
        OnHit?.Invoke(hitInfo);
    }

    public void SetInvulnerable(float invulnerabilityTime)
    {
        if (invulnerable)
        {
            return;
        }

        invulnerable = true;
        invulnerabilityDuration = invulnerabilityTime;
    }
}
