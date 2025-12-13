using System;
using UnityEngine;

public class ProgramInputManager : MonoBehaviour
{
    private const KeyCode attackKey = KeyCode.Mouse0;
    private const KeyCode defenseKey = KeyCode.Mouse1;
    private const KeyCode dashKey = KeyCode.Space;
    private const KeyCode timeSlowKey = KeyCode.Tab;

    public bool canUseProgram;
    public bool isAttacking;
    public bool isDefending;
    public bool isDashing;
    public bool inSlowTimeMode;

    //In each program logic script, the main attack method (such as Slash() in the SlashLogic script) 
    //will be subscribed to the StartAttackProgram event on Start() in that program logic script.
    public event Action StartAttackProgram;
    public event Action StartDefenseProgram;
    public event Action StartDash;

    //These events are to be subscribed to by animations or effects that need to begin or end upon
    //slow mode enter and exit
    public event Action OnSlowModeEnter;
    public event Action OnSlowModeExit;

    private TimeSlowTimerLogic timeSlowTimerLogic;
    private DashChargeManager dashChargeManager;
    private ProgramListData attackProgramList;
    private ProgramListData defenseProgramList;

    void Start()
    {
        attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
        defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();
        dashChargeManager = FindObjectOfType<DashChargeManager>();
        timeSlowTimerLogic = FindObjectOfType<TimeSlowTimerLogic>();
    }

    public void ForceExitSlowMode()
    {
        inSlowTimeMode = false;

        if (isAttacking == false && isDefending == false)
        {
            canUseProgram = true;
        }
        else
        {
            canUseProgram = false;
        }
    }

    void Update()
    {

        if (canUseProgram == false && isAttacking == false && isDefending == false 
        && isDashing == false && inSlowTimeMode == false)
        {
            canUseProgram = true;
        }

        if (canUseProgram)
        {
            if (Input.GetKeyDown(attackKey) && attackProgramList.AreProgramsAvailable())
            {
                StartAttackProgram?.Invoke();
                isAttacking = true;
                canUseProgram = false;
            }

            if (Input.GetKeyDown(defenseKey) && defenseProgramList.AreProgramsAvailable())
            {
                StartDefenseProgram?.Invoke();
                isDefending = true;
                canUseProgram = false;
            }

            if (Input.GetKeyDown(dashKey) && dashChargeManager.IsDashChargeAvailable())
            {
                StartDash?.Invoke();
                isDashing = true;
                canUseProgram = false;
            }       
        }

        if (Input.GetKeyDown(timeSlowKey) && timeSlowTimerLogic.IsTimeSlowAboveZero())
        {
            Debug.Log("Entering program rearrangement mode");
            OnSlowModeEnter?.Invoke();
            canUseProgram = false;
            inSlowTimeMode = true;
        }
        else if (Input.GetKeyUp(timeSlowKey))
        {
            Debug.Log("Exiting program rearrangement mode");
            OnSlowModeExit?.Invoke();
            ForceExitSlowMode();
        }
    }
    
}
