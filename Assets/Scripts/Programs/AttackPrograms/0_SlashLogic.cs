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
    }

    public override void FireProgram(QueueParameter queueParameter)
    {
        Slash(queueParameter);
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
    }
}
