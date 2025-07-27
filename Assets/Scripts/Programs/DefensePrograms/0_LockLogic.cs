public class LockLogic : Program
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
            inputManager.StartDefenseProgram += Lock;
        }
    }

    void Lock()
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Defense);
    }

    void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram -= Lock;
        }
    }
}
