using UnityEngine;

public class LockLogic : Program
{
    private const int hitBoxDeactivationFrame = 5;
    private const int hitBoxActivationFrame = 10;

    void Start()
    {
        FindDependencies();

        if (playerAnimator != null && programUI != null)
        {
            playerAnimator.OnAnimationComplete += OnAnimationCompleted;
            playerAnimator.OnFrameChanged += DisableHitbox;
            playerAnimator.OnFrameChanged += EnableHitbox;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram += Lock;
        }
    }

    void Lock()
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
            playerAnimator.OnAnimationComplete -= OnAnimationCompleted;
            playerAnimator.OnFrameChanged -= DisableHitbox;
            playerAnimator.OnFrameChanged -= EnableHitbox;
        }

        if (inputManager != null)
        {
            inputManager.StartDefenseProgram -= Lock;
        }
    }
}
