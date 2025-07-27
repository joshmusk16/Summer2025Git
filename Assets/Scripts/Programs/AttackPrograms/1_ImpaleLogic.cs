public class ImpaleLogic : Program
{
    void Start()
    {
        FindDependencies();

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete += OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram += Impale;
        }
    }

    void Impale()
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
    }

    void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= Impale;
        }
    }
}
