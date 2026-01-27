public class ImpaleLogic : Program
{
    void Awake()
    {
        FindDependencies();
    }

    public override void FireProgram(QueueParameter queueParameter)
    {
        Impale(queueParameter);
    }
    
    void Impale(QueueParameter queueParameter)
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
    }
}
