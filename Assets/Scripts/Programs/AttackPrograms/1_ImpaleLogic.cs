public class ImpaleLogic : Program
{
    public override void FireProgram(QueueParameter queueParameter)
    {
        Impale(queueParameter);
    }
    
    void Impale(QueueParameter queueParameter)
    {
        AssignHitboxDamages(ComboBarLogic.currentCombo + 2);
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
        ChangeTransform(queueParameter.facedDirection);
    }
}
