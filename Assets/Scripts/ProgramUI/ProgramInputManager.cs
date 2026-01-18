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
    private ProgramUI attackProgramUI;
    private ProgramUI defenseProgramUI;

    private QueueListData queueProgramList;

    //temporary, ultimately all queueDataCollection is called from QueueDataList
    private QueueDataCollector queueDataCollector;
    public GameObject dashChargeFiring;

    void Start()
    {
        attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
        attackProgramUI = GameObject.Find("AttackUIManager").GetComponent<ProgramUI>();

        defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();
        defenseProgramUI = GameObject.Find("DefenseUIManager").GetComponent<ProgramUI>();

        queueProgramList = FindObjectOfType<QueueListData>();
        dashChargeManager = FindObjectOfType<DashChargeManager>();
        timeSlowTimerLogic = FindObjectOfType<TimeSlowTimerLogic>();

        //temporary, ultimately all queueDataCollection is called from QueueDataList
        queueDataCollector = FindObjectOfType<QueueDataCollector>();
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
            if (Input.GetKeyDown(ATTACK_KEY) && attackProgramList.AreProgramsAvailable())
            {
                if(queueProgramList.queueList.Count == 0)
                {
                    //Temporary code, just making a point that the queueParamter should pass through the new FireProgram method in Program class
                    //Eventually the firing will be called in QueueListData when it gets set up correctly
                    QueueParameter queueParameter = queueDataCollector.CollectQueueData(attackProgramList.currentProgram, ProgramType.Attack);
                    attackProgramList.currentProgram.GetComponent<Program>().FireProgram(queueParameter);
                    isAttacking = true;
                    canUseProgram = false;    
                }
                else
                {
                    attackProgramUI.UpdateQueueUIOnClick(); 
                    //queueProgramList.AddProgramToQueue(ProgramType.Attack);
                    //this will probably move to be called elsewhere after the queue 
                    //is updated in QueueDataCollector / QueueListData
                }
            }

            if (Input.GetKeyDown(DEFENSE_KEY) && defenseProgramList.AreProgramsAvailable())
            {
                //Temporary code, just making a point that the queueParamter should pass through the new FireProgram method in Program class
                //Eventually the firing will be called in QueueListData when it gets set up correctly
                QueueParameter queueParameter = queueDataCollector.CollectQueueData(defenseProgramList.currentProgram, ProgramType.Defense);
                defenseProgramList.currentProgram.GetComponent<Program>().FireProgram(queueParameter);

                isDefending = true;
                canUseProgram = false;

                //queueProgramList.AddProgramToQueue(ProgramType.Defense);
            }

            if (Input.GetKeyDown(DASH_KEY) && dashChargeManager.IsDashChargeAvailable())
            {
                QueueParameter queueParameter = queueDataCollector.CollectQueueData(dashChargeFiring, ProgramType.Dash);
                dashChargeFiring.GetComponent<Program>().FireProgram(queueParameter);
                isDashing = true;
                canUseProgram = false;

                //queueProgramList.AddProgramToQueue(ProgramType.Dash);
            }       
        }

        if (Input.GetKeyDown(TIMESLOW_KEY) && timeSlowTimerLogic.IsTimeSlowAboveZero())
        {
            Debug.Log("Entering program rearrangement mode");
            OnSlowModeEnter?.Invoke();
            canUseProgram = false;
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
