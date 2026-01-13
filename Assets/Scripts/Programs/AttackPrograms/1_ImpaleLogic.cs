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
            StartProgram += Impale;
        }
    }

    void Impale(QueueParameter queueParameter)
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
            StartProgram -= Impale;
        }
    }
}
