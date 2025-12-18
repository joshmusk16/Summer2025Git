using UnityEngine;

public class ProgramTimerUI : MonoBehaviour
{

    private CustomAnimator playerAnimator;
    private ProgramInputManager inputManager;

    public GameObject animBar;
    public bool updatingBar = false;

    public ProgramType programType;

    //Fetch dependencies
    void Start()
    {
        inputManager = FindObjectOfType<ProgramInputManager>();

        if (inputManager != null)
        {
            if (programType == ProgramType.Attack)
            {
                inputManager.StartAttackProgram += StartUpdatingBar;
            }
            else if (programType == ProgramType.Defense)
            {
                inputManager.StartDefenseProgram += StartUpdatingBar;
            }
        }

        GameObject playerLogic = FindObjectOfType<PlayerLogic>().gameObject;

        if (playerLogic != null)
        {
            playerAnimator = playerLogic.GetComponent<CustomAnimator>();
        }
    }

    void Update()
    {
        if (updatingBar)
        {
            UpdateAttackTimerBar();
        }
    }

    //Logic to animate the attack timer ui bar
    void UpdateAttackTimerBar()
    {
        bool isActive = (programType == ProgramType.Attack && inputManager.isAttacking) ||
        (programType == ProgramType.Defense && inputManager.isDefending);
    
        if (isActive)
        {
            float progress = playerAnimator.GetAnimationProgress();
            animBar.transform.localScale = new Vector2(QuadraticEaseInWithHold(progress, 0.8f), 1f);
        }
        else
        {
            StopUpdatingBar(programType);
        }
    }

    //Call to visualize begin animating attack timer ui bar in Update()
    public void StartUpdatingBar()
    {
        if (updatingBar) return;

        updatingBar = true;
        playerAnimator.OnAnimationComplete += StopUpdatingBar;
    }

    //Note this will not unsubscribe if a looping animation begins
    public void StopUpdatingBar(ProgramType type)
    {
        updatingBar = false;
        animBar.transform.localScale = new Vector2(0f, 1f);
        playerAnimator.OnAnimationComplete -= StopUpdatingBar;
        //Debug.Log("Unsubscribing Stop Updating");
    }

    //Handle unsubscriptions
    void OnDestroy()
    {
        if (inputManager != null)
        {
            inputManager.StartAttackProgram -= StartUpdatingBar;
            inputManager.StartDefenseProgram -= StartUpdatingBar;
        }

        if (playerAnimator != null)
        {
            playerAnimator.OnAnimationComplete -= StopUpdatingBar;
        }
    }

    //Revieves a input of 0-1 as linearProgress and modifies in quadratically with a specified delay
    //If there ends up being a lot of easing method taking 0-1 linear inputs, we should move them to a single class
    public static float QuadraticEaseInWithHold(float linearProgress, float completionThreshold = 0.8f)
    {
        linearProgress = Mathf.Clamp01(linearProgress);
        completionThreshold = Mathf.Clamp(completionThreshold, 0.1f, 1f);

        // If we're past the completion threshold, stay at 1.0 (dramatic pause)
        if (linearProgress >= completionThreshold)
            return 1.0f;

        // Scale the progress so it reaches the full 0-1 range by the threshold
        float scaledProgress = linearProgress / completionThreshold;

        // Apply quadratic easing to the scaled progress
        return scaledProgress * scaledProgress;
    }

}
