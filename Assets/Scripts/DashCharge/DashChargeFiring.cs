public class DashChargeFiring : Program
{
    private const float DASH_SPEED = 15f;
    private const int REMOVE_CHARGE_AMOUNT = 1;

    private DashChargeManager dashChargeManager;

    void Start()
    {
        FindDependencies();
        dashChargeManager = FindAnyObjectByType<DashChargeManager>();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<CustomAnimator>();
        }

        if(inputManager != null)
        {
            StartProgram += Dash;
        }
    }

    public void Dash(QueueParameter queueParameter)
    {
        playerMovement.MovePlayerLerp(queueParameter.destination, DASH_SPEED);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Dash, () => playerMovement.PlayerLerpProgress(), false);
    }

    public void OnDashCompleted(ProgramType type)
    {
        if(type == ProgramType.Dash)
        {
        inputManager.isDashing = false;
        dashChargeManager.RemoveDashCharge(REMOVE_CHARGE_AMOUNT);
        }
    }

    void OnDestroy()
    {
        StartProgram -= Dash;
    }
}
