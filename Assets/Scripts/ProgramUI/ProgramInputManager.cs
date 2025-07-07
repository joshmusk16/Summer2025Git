using System;
using UnityEngine;

public class ProgramInputManager : MonoBehaviour
{
    public bool canAttack = true;

    //In each program logic script, the main attack method (such as Slash() in the SlashLogic script) 
    //will be subscribed to the StartAttackProgram event on Start() in that program logic script.
    public event Action StartAttackProgram;
    public event Action StartDefenseProgram;

    void Update()
    {

        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartAttackProgram?.Invoke();
                canAttack = false;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartDefenseProgram?.Invoke();
                canAttack = false;
            }   
        }
    }
    
}
