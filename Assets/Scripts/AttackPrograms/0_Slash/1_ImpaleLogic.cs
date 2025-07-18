public class ImpaleLogic : Program
{
    void Start()
    {
        FindDependencies();

        if (playerAnimator != null && attackUI != null)
        {
            playerAnimator.OnAnimationComplete += attackUI.ScrollAttackUI;
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
        if (playerAnimator != null && attackUI != null)
        {
            playerAnimator.OnAnimationComplete -= attackUI.ScrollAttackUI;
        }

        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= Impale;
        }
    }
}
