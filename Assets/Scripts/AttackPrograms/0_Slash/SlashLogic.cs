using UnityEngine;

public class SlashLogic : MonoBehaviour
{
    private PlayerLogic player;
    private Animator playerAnimator;

    private AttackUI attackUI;

    public Sprite[] slashSprites;
    public float[] slashFrames;

    public HitBox[] slashHitboxes;

    private bool hasUsed = false;

    void Start()
    {
        attackUI = FindObjectOfType<AttackUI>();
        player = FindObjectOfType<PlayerLogic>();
        playerAnimator = player.gameObject.GetComponent<Animator>();

        if (player != null)
        {
            player.MouseLeftOrRightChanged += ChangeTransform;
            ChangeTransform(player.currentMouseLeftOrRight);
        }

        if (playerAnimator != null && attackUI != null)
        {
            playerAnimator.OnAnimationComplete += attackUI.ScrollAttackUI;
        }
    }

    //update for debugging only
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && hasUsed == false)
        {
            Slash();
            hasUsed = true;
        }
    }

    void Slash()
    {
        playerAnimator.PlayAnimation(slashSprites, slashFrames, false, true, slashHitboxes, 3);
    }

    void ChangeTransform(int direction)
    {
        foreach (HitBox hitbox in slashHitboxes)
        {
            if (direction == 1)
            {
                hitbox.offset.x = Mathf.Abs(hitbox.offset.x);
            }
            else if (direction == -1)
            {
                hitbox.offset.x = -Mathf.Abs(hitbox.offset.x);
            }
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.MouseLeftOrRightChanged -= ChangeTransform;    
        }

        if (playerAnimator != null && attackUI != null)
        {
            playerAnimator.OnAnimationComplete -= attackUI.ScrollAttackUI;   
        }
    }
}
