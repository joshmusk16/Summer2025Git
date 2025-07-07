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

        if (inputManager != null)
        {
            inputManager.StartAttackProgram += Slash;
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

        //reassigning inputManager.canAttack to true here might not be best practice
        if (inputManager != null)
        {
            inputManager.canAttack = true;
            inputManager.StartAttackProgram -= Slash;
        }
        
    }
}
