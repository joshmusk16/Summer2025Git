using UnityEngine;

public class DashChargeFiring : MonoBehaviour
{
    private const float dashSpeed = 20f;
    private const int dashRange = 4;
    private const int removeChargeAmount = 1;

    private ProgramInputManager programInputManager;
    private PlayerMovement playerMovement;
    private CustomAnimator playerAnimator;
    private PlayerTargeting playerTargeting;
    private PlayerLogic player;
    private DashChargeManager dashChargeManager;

    [Header("Animation Data")]
    public Sprite[] animSprites;
    public float[] animFrames;

    void Start()
    {
        programInputManager = FindAnyObjectByType<ProgramInputManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerTargeting = FindObjectOfType<PlayerTargeting>();
        player = FindObjectOfType<PlayerLogic>();
        dashChargeManager = FindAnyObjectByType<DashChargeManager>();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<CustomAnimator>();
        }

        if (playerAnimator != null)
        {
            playerAnimator.OnAnimationComplete += OnDashCompleted;
        }

        if(programInputManager != null)
        {
            programInputManager.StartDash += Dash;
        }
    }

    public void Dash()
    {
        playerMovement.MovePlayerLerp(playerTargeting.SelectedTile(dashRange), dashSpeed);
        playerAnimator.PlayParameterDrivenAnimation(animSprites, animFrames, ProgramType.Dash, () => playerMovement.PlayerLerpProgress(), false);
    }

    void OnDashCompleted(ProgramType type)
    {
        if(type == ProgramType.Dash)
        {
        programInputManager.isDashing = false;
        dashChargeManager.RemoveDashCharge(removeChargeAmount);   
        }
    }

    void OnDestroy()
    {
        playerAnimator.OnAnimationComplete -= OnDashCompleted;
        programInputManager.StartDash -= Dash;
    }
}
