public class SlashLogic : Program
{
    public override void FireProgram(QueueParameter queueParameter)
    {
        Slash(queueParameter);
    }

    void Slash(QueueParameter queueParameter)
    {
        AssignHitboxDamages(comboBar.currentCombo);
        playerAnimator.PlayAnimation(animSprites, animFrames, ProgramType.Attack, false, true, hitboxTimings);
        ChangeTransform(queueParameter.facedDirection);
    }
}