using UnityEngine;

[System.Serializable]

public class HitInfo
{
    public HitBox hitbox;
    public HurtBox hurtbox;
    public int damage;
    public float knockback;
    public Vector2 knockbackDirection;
    public float hitstun;
    public Vector2 hitPoint;
}

