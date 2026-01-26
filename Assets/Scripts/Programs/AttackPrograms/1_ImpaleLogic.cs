public class ImpaleLogic : Program
{
    void Awake()
    {
        FindDependencies();

        if (inputManager != null)
        {
            StartProgram += Impale;
        }
    }

    void Impale(QueueParameter queueParameter)
    {
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
    }

    void OnDestroy()
    {
        if (inputManager != null)
        {
            StartProgram -= Impale;
        }
    }
}
