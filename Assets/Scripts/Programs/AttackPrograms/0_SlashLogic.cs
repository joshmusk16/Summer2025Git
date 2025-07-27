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
            playerAnimator.OnAnimationComplete += OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram += Slash;
        }
    }

    void Slash()
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.MouseLeftOrRightChanged -= ChangeTransform;
        }

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= Slash;
        }
    }
}
