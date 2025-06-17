
using System.Collections.Generic;
using UnityEngine;

public class HitBox : CollisionBox
{

    [Header("Hitbox Properties")]
    public int damage = 1;
    public float hitstun = 0.2f;
    public string[] hitboxTags = { "default" };
    
    [Header("Hitbox Behavior")]
    public bool multiHit = false;
    public float multiHitInterval = 0.1f;
    public int maxHits = -1; // -1 for unlimited

    private Dictionary<HurtBox, float> lastHitTimes = new();
    private Dictionary<HurtBox, int> hitCounts = new();


}


