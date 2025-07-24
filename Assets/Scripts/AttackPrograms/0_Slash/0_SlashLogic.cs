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

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete += programUI.ScrollProgramUI;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram += Slash;
        }
    }

    void Slash()
    {
        playerAnimator.PlayAnimation(attackSprites, attackFrames, false, true, hitboxTimings);
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.MouseLeftOrRightChanged -= ChangeTransform;
        }

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= programUI.ScrollProgramUI;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= Slash;
        }
    }
}
