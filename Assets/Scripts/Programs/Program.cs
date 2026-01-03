using UnityEngine;    

    [System.Serializable]
    public struct HitboxTiming
    {
        public HitBox hitbox;
        public int[] activationFrames;
    }

public class Program : MonoBehaviour
{
    [Header("Program Type")]
    public ProgramType programType = ProgramType.Attack;
    public Sprite uiSprite = null;
    public bool isMovementProgram = false;

    [Header("Animation Data")]
    public Sprite[] animSprites;
    public float[] animFrames;
    public HitboxTiming[] hitboxTimings;

    [HideInInspector] public PlayerLogic player;
    [HideInInspector] public CustomAnimator playerAnimator;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerTargeting playerTargeting;
    [HideInInspector] public ProgramUI programUI;
    [HideInInspector] public ProgramInputManager inputManager;
    
    //In script for any program inheriting this class, run FindDependencies() in Start()
    protected virtual void FindDependencies()
    {
        //Be aware that changing the AttackUIManager name in the editor will break GameObject.Find()
        if (programType == ProgramType.Attack)
        {
            programUI = GameObject.Find("AttackUIManager").GetComponent<ProgramUI>();
        }
        else if (programType == ProgramType.Defense)
        {
            programUI = GameObject.Find("DefenseUIManager").GetComponent<ProgramUI>();
        }

        player = FindObjectOfType<PlayerLogic>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerTargeting = FindObjectOfType<PlayerTargeting>();
        inputManager = FindObjectOfType<ProgramInputManager>();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<CustomAnimator>();
        }
    }

    protected virtual void OnAnimationCompleted(ProgramType completedType)
    {
        // Only scroll if this animation type matches our program type
        if (completedType == programType && programUI != null)
        {
            programUI.ScrollOrSetupNewHand();
        }
    }

    //When given a direction of -1 or 1 in in direction, this method will change the transform of the hitboxes 
    //appropriately. This is intended to be subscribed to MouseLeftOrRightOfPlayer() in the PlayerHandler script
    //See SlashLogic Script for reference
    protected virtual void ChangeTransform(int direction)
    {
        foreach (HitboxTiming timing in hitboxTimings)
        {
            HitBox temp = timing.hitbox;

            if (direction == 1)
            {
                temp.offset.x = Mathf.Abs(temp.offset.x);
            }
            else if (direction == -1)
            {
                temp.offset.x = -Mathf.Abs(temp.offset.x);
            }
        }
    }
}
