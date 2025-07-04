using UnityEngine;

public class SlashLogic : Program
{

    void Start()
    {
        FindDependencies();

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
        playerAnimator.PlayAnimation(attackSprites, attackFrames, false, true, attackHitboxes, 3);
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
