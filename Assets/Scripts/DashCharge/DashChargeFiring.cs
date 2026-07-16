public class DashChargeFiring : Program
{
    private const float DASH_SPEED = 15f;

    void Start()
    {
        FindDependencies();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<CustomAnimator>();
        }
    }

    public override void FireProgram(QueueParameter queueParameter)
    {
        Dash(queueParameter);
    }

    public void Dash(QueueParameter queueParameter)
    {
        playerMovement.MovePlayerLerp(queueParameter.destination, DASH_SPEED);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Dash, () => playerMovement.PlayerLerpProgress(), false);
        ChangeTransform(queueParameter.facedDirection);
    }

    public void OnDashCompleted(ProgramType type)
    {
        if(type == ProgramType.Dash)
        {
        inputManager.isDashing = false;
        }
    }
}
