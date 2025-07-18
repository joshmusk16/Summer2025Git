using UnityEngine;

public class AttackTimerUI : MonoBehaviour
{

    private Animator playerAnimator;
    private ProgramInputManager inputManager;

    public GameObject animBar;
    public bool updatingBar = false;

    //Fetch dependencies
    void Start()
    {
        inputManager = FindObjectOfType<ProgramInputManager>();

        if (inputManager != null)
        {
            inputManager.StartAttackProgram += StartUpdatingBar;
        }

        GameObject playerLogic = FindObjectOfType<PlayerLogic>().gameObject;

        if (playerLogic != null)
        {
            playerAnimator = playerLogic.GetComponent<Animator>();
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
        if (inputManager.isAttacking)
        {
            float temp = playerAnimator.GetAnimationProgress();
            animBar.transform.localScale = new Vector2(QuadraticEaseInWithHold(temp, 0.5f), 1f);
        }
        else
        {
            StopUpdatingBar();
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
    public void StopUpdatingBar()
    {
        updatingBar = false;
        animBar.transform.localScale = new Vector2(0f, 1f);
        playerAnimator.OnAnimationComplete -= StopUpdatingBar;
        Debug.Log("Unsubscribing Stop Updating");
    }

    //Handle unsubscriptions
    void OnDestroy()
    {
        inputManager.StartAttackProgram -= StartUpdatingBar;
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
