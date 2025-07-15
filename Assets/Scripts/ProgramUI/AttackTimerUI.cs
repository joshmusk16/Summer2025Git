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
            animBar.transform.localScale = new Vector2(playerAnimator.GetAnimationProgress(), 1f);
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

}
