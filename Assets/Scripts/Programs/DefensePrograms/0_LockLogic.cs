public class LockLogic : Program
{
    void Start()
    {
        FindDependencies();

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete += programUI.ScrollProgramUI;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram += Lock;
        }
    }

    void Lock()
    {
        playerAnimator.PlayAnimation(animSprites, animFrames);
    }

    void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= programUI.ScrollProgramUI;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram -= Lock;
        }
    }
}
