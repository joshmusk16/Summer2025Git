using UnityEngine;

public class DummyLogic : MonoBehaviour
{

    public Sprite[] OnhitAnim;
    public float[] animFrames;
    public Sprite idleSprite;

    public HurtBox hurtbox;
    public Animator animator;
    public EnemyHealthBar health;

    void Start()
    {
        if (hurtbox != null)
        {
            hurtbox.OnHit += StartHitAnimation;
            hurtbox.OnHit += health.DamageHealthBar;
        }
    }

    private void StartHitAnimation(HitInfo hitInfo)
    {
        animator.PlayAnimation(OnhitAnim, animFrames, ProgramType.Other, false);
    }

    void OnDestroy()
    {
        hurtbox.OnHit -= StartHitAnimation;
        hurtbox.OnHit -= health.DamageHealthBar;
    }
}
