public class SlashLogic : Program
{

    void Awake()
    {
        FindDependencies();

        if (player != null)
        {
            player.MouseLeftOrRightChanged += ChangeTransform;
            ChangeTransform(player.currentMouseLeftOrRight);
        }

        if (inputManager != null)
        {
            StartProgram += Slash;
        }
    }

    void Slash(QueueParameter queueParameter)
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.MouseLeftOrRightChanged -= ChangeTransform;
        }

        if (inputManager != null)
        {
            StartProgram -= Slash;
        }
    }
}
