using System;
using UnityEngine;

public class ProgramInputManager : MonoBehaviour
{
    private const KeyCode ATTACK_KEY = KeyCode.Mouse0;
    private const KeyCode DEFENSE_KEY = KeyCode.Mouse1;
    private const KeyCode DASH_KEY = KeyCode.Space;
    private const KeyCode TIMESLOW_KEY = KeyCode.Tab;

    public bool canUseProgram = true;
    private bool inputIsAllowed = true;
    public bool isAttacking;
    public bool isDefending;
    public bool isDashing;
    public bool inSlowTimeMode;

    //These events are to be subscribed to by animations or effects that need to begin or end upon
    //slow mode enter and exit
    public event Action OnSlowModeEnter;
    public event Action OnSlowModeExit;

    private ProgramListData attackProgramList;
    private ProgramListData defenseProgramList;
    private PanelsUI panelsUI;

    private QueueListData queueProgramList;

    public GameObject dashChargeFiring;

    void Start()
    {
        attackProgramList = GameObject.Find("AttackUIManager").GetComponent<ProgramListData>();
        defenseProgramList = GameObject.Find("DefenseUIManager").GetComponent<ProgramListData>();

        queueProgramList = FindObjectOfType<QueueListData>();
        panelsUI = FindObjectOfType<PanelsUI>();
    }

    public void ForceExitSlowMode()
    {
        OnSlowModeExit?.Invoke();
        inSlowTimeMode = false;
        canUseProgram = true;
    }

    void Update()
    {
        if (panelsUI.isInLeftOrRightZone == true && inSlowTimeMode == true)
        {
            canUseProgram = false;
        }
        else
        {
            canUseProgram = true;
        }

        if (canUseProgram && inputIsAllowed)
        {
            if (Input.GetKeyDown(ATTACK_KEY) && attackProgramList.AreProgramsAvailable())
            {
                queueProgramList.AddProgramToQueue(ProgramType.Attack);
                isAttacking = true;
            }

            if (Input.GetKeyDown(DEFENSE_KEY) && defenseProgramList.AreProgramsAvailable())
            {
                queueProgramList.AddProgramToQueue(ProgramType.Defense);
                isDefending = true;
            }

            if (Input.GetKeyDown(DASH_KEY))
            {
                queueProgramList.AddProgramToQueue(ProgramType.Dash);
                isDashing = true;
            }       
        }

        if (Input.GetKeyDown(TIMESLOW_KEY))
        {
            Debug.Log("Entering program rearrangement mode");
            OnSlowModeEnter?.Invoke();
            inSlowTimeMode = true;
        }
        else if (Input.GetKeyUp(TIMESLOW_KEY))
        {
            Debug.Log("Exiting program rearrangement mode");
            ForceExitSlowMode();
        }
    }

    public void DisableInput()
    {
        inputIsAllowed = false;
    }

    public void EnableInput()
    {
        inputIsAllowed = true;
    }
    
}
