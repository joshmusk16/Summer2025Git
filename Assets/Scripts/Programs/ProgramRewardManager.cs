using UnityEngine;

public class ProgramRewardManager : MonoBehaviour
{

private ComboBarLogic comboBar;
private PlayerTimerLogic playerTimerBar;
private TimeManager gameSpeedManager;

private void FindDependencies()
{
    if(comboBar == null) comboBar = FindObjectOfType<ComboBarLogic>();
    if(playerTimerBar == null) playerTimerBar = FindObjectOfType<PlayerTimerLogic>();
    if(gameSpeedManager == null) gameSpeedManager = FindObjectOfType<TimeManager>();
}

public void GiveRewards(Program programData)
{  
    FindDependencies();

    comboBar.ChangeComboBar(programData.comboRewardType, programData.amountToChangeComboBar);
    playerTimerBar.ChangeTimerBar(programData.timerRewardType, programData.amountToChangeTimerBar);
    gameSpeedManager.ChangeGameSpeed(programData.gameSpeedRewardType, programData.amountToChangeGameSpeed);
    programData.ConditionMet();
}

}
