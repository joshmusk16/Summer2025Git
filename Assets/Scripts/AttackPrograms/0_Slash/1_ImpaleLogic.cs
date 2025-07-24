public class ImpaleLogic : Program
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
            inputManager.StartAttackProgram += Impale;
        }
    }

    void Impale()
    {
        playerAnimator.PlayAnimation(attackSprites, attackFrames, false, true, hitboxTimings);
    }

        void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= programUI.ScrollProgramUI;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= Impale;
        }
    }
}
