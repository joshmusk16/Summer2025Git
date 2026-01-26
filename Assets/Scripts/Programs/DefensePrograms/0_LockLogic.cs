public class LockLogic : Program
{
    private const int hitBoxDeactivationFrame = 5;
    private const int hitBoxActivationFrame = 10;

    void Awake()
    {
        FindDependencies();

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnFrameChanged += DisableHitbox;
            playerAnimator.OnFrameChanged += EnableHitbox;
        }

        if (inputManager != null)
        {
            StartProgram += Lock;
        }
    }

    void Lock(QueueParameter queueParameter)
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Defense);
    }

    void DisableHitbox(int frame, ProgramType type)
    {
        if (frame == hitBoxDeactivationFrame && type == programType) player.DisablePlayerHitbox();
    }

    void EnableHitbox(int frame, ProgramType type)
    {
        if (frame == hitBoxActivationFrame && type == programType) player.EnablePlayerHitbox();
    }

    void OnDestroy()
    {
        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnFrameChanged -= DisableHitbox;
            playerAnimator.OnFrameChanged -= EnableHitbox;
        }

        if (inputManager != null)
        {
            StartProgram -= Lock;
        }
    }
}
