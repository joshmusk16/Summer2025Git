public class DashChargeFiring : Program
{
    private const float DASH_SPEED = 20f;
    private const int DASH_RANGE = 4;
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

        if (playerAnimator != null)
        {
            playerAnimator.OnAnimationComplete += OnDashCompleted;
        }

        if(inputManager != null)
        {
            inputManager.StartDash += Dash;
        }
    }

    public void Dash()
    {
        playerMovement.MovePlayerLerp(playerTargeting.SelectedTile(DASH_RANGE), DASH_SPEED);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Dash, () => playerMovement.PlayerLerpProgress(), false);
    }

    void OnDashCompleted(ProgramType type)
    {
        if(type == ProgramType.Dash)
        {
        inputManager.isDashing = false;
        dashChargeManager.RemoveDashCharge(REMOVE_CHARGE_AMOUNT);   
        }
    }

    void OnDestroy()
    {
        playerAnimator.OnAnimationComplete -= OnDashCompleted;
        inputManager.StartDash -= Dash;
    }
}
