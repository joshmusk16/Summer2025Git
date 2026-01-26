public class DashLogic : Program
{
    private const float dashSpeed = 20f;
    private const int dashRange = 4;

    void Awake()
    {
        FindDependencies();

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram += Dash;
        }
    }

    void Dash()
    {
        playerMovement.MovePlayerLerp(playerTargeting.SelectedTile(dashRange), dashSpeed);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Defense, () => playerMovement.PlayerLerpProgress(), false);
    }

    void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.StartDefenseProgram -= Dash;
        }
    }
}