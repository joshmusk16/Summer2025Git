public class SlashLogic : Program
{
    void Awake()
    {
        FindDependencies();
    }

    public override void FireProgram(QueueParameter queueParameter)
    {
        Slash(queueParameter);
    }

    void Slash(QueueParameter queueParameter)
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
        ChangeTransform(queueParameter.facedDirection);
    }
}