using System;
using UnityEngine;

public class ProgramInputManager : MonoBehaviour
{
    private const KeyCode ATTACK_KEY = KeyCode.Mouse0;
    private const KeyCode DEFENSE_KEY = KeyCode.Mouse1;
    private const KeyCode DASH_KEY = KeyCode.Space;
    private const KeyCode TIMESLOW_KEY = KeyCode.Tab;

    public bool canUseProgram;
    public bool isAttacking;
    public bool isDefending;
    public bool isDashing;
    public bool inSlowTimeMode;

    //These events are to be subscribed to by animations or effects that need to begin or end upon
    //slow mode enter and exit
    public event Action OnSlowModeEnter;
    public event Action OnSlowModeExit;

    private TimeSlowTimerLogic timeSlowTimerLogic;
    private DashChargeManager dashChargeManager;
    private ProgramListData attackProgramList;
    private ProgramListData defenseProgramList;

    private QueueListData queueProgramList;

    public GameObject dashChargeFiring;

    void Start()
    {
        attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
        defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();

        queueProgramList = FindObjectOfType<QueueListData>();
        dashChargeManager = FindObjectOfType<DashChargeManager>();
        timeSlowTimerLogic = FindObjectOfType<TimeSlowTimerLogic>();
    }

    public void ForceExitSlowMode()
    {
        inSlowTimeMode = false;
        canUseProgram = false;
    }

    void Update()
    {

        //Must add parameters here that track whether or not a attack vs defense program can be queued

        if (inSlowTimeMode == false)
        {
            canUseProgram = true;
        }

        if (canUseProgram)
        {
            if (Input.GetKeyDown(ATTACK_KEY) && attackProgramList.AreProgramsAvailable())
            {
                queueProgramList.AddProgramToQueue(ProgramType.Attack);
                isAttacking = true;
                //canUseProgram = false;    
            }

            if (Input.GetKeyDown(DEFENSE_KEY) && defenseProgramList.AreProgramsAvailable())
            {
                queueProgramList.AddProgramToQueue(ProgramType.Defense);
                isDefending = true;
                //canUseProgram = false;
            }

            if (Input.GetKeyDown(DASH_KEY) && dashChargeManager.IsDashChargeAvailable())
            {
                queueProgramList.AddProgramToQueue(ProgramType.Dash);
                isDashing = true;
                //canUseProgram = false;
            }       
        }

        if (Input.GetKeyDown(TIMESLOW_KEY) && timeSlowTimerLogic.IsTimeSlowAboveZero())
        {
            Debug.Log("Entering program rearrangement mode");
            OnSlowModeEnter?.Invoke();
            inSlowTimeMode = true;
        }
        else if (Input.GetKeyUp(TIMESLOW_KEY))
        {
            Debug.Log("Exiting program rearrangement mode");
            OnSlowModeExit?.Invoke();
            ForceExitSlowMode();
        }
    }
    
}
