public class DashLogic : Program
{
    private const float dashSpeed = 10f;

    void Start()
    {
        FindDependencies();

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete += OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram += Dash;
        }
    }

    void Dash()
    {
        playerMovement.MovePlayerLerp(player.mouseTracker.GetWorldMousePosition(), dashSpeed);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Defense, () => playerMovement.PlayerLerpProgress(), false);
    }

    void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete -= OnAnimationCompleted;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram -= Dash;
        }
    }
}