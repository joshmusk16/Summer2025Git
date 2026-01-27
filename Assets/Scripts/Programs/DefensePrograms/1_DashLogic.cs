public class DashLogic : Program
{
    private const float dashSpeed = 20f;
    private const int dashRange = 4;

    void Awake()
    {
        FindDependencies();
    }

    public override void FireProgram(QueueParameter queueParameter)
    {
        Dash(queueParameter);
    }

    void Dash(QueueParameter queueParameter)
    {
        playerMovement.MovePlayerLerp(playerTargeting.SelectedTile(dashRange), dashSpeed);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Defense, () => playerMovement.PlayerLerpProgress(), false);
    }
}