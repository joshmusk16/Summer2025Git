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

    void Update()
    {

        if (canUseProgram == false && isAttacking == false && inSlowTimeMode == false)
        {
            canUseProgram = true;
        }

        if (canUseProgram)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartAttackProgram?.Invoke();
                canUseProgram = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartDefenseProgram?.Invoke();
                canUseProgram = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Entering program rearrangement mode");
            canUseProgram = false;
            inSlowTimeMode = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Exiting program rearrangement mode");
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
    }
    
}
