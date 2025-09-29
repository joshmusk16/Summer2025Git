using System;
using UnityEngine;

public class ProgramInputManager : MonoBehaviour
{
    public bool canUseProgram;
    public bool isAttacking;
    public bool isDefending;
    public bool inSlowTimeMode;

    //In each program logic script, the main attack method (such as Slash() in the SlashLogic script) 
    //will be subscribed to the StartAttackProgram event on Start() in that program logic script.
    public event Action StartAttackProgram;
    public event Action StartDefenseProgram;

    //These events are to be subscribed to by animations or effects that need to begin or end upon
    //slow mode enter and exit
    public event Action OnSlowModeEnter;
    public event Action OnSlowModeExit;

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

        if (canUseProgram == false && isAttacking == false && isDefending == false && inSlowTimeMode == false)
        {
            canUseProgram = true;
        }

        if (canUseProgram)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartAttackProgram?.Invoke();
                isAttacking = true;
                canUseProgram = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartDefenseProgram?.Invoke();
                isDefending = true;
                canUseProgram = false;
            }       
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Entering program rearrangement mode");
            OnSlowModeEnter?.Invoke();
            canUseProgram = false;
            inSlowTimeMode = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Exiting program rearrangement mode");
            OnSlowModeExit?.Invoke();
            ForceExitSlowMode();
        }
    }
    
}
