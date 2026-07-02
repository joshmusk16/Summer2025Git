using System;
using UnityEngine;    

    [Serializable]
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

    [Header("Player Targeting Parameters")]
    public bool isMovementProgram = false;
    public int targetingRange = 0;

    [Header("Animation Data")]
    public Sprite[] animSprites;
    public float[] animFrames;
    public HitboxTiming[] hitboxTimings;

    [Header("Reward Data")]
    public int rewardRequirementType;

    public int comboRewardType;
    public int amountToChangeComboBar;

    public int timerRewardType;
    public int amountToChangeTimerBar;
    
    //Reward Requirement types : 1 = HasAnyHitboxHit, 2 = HasEveryHitboxHit
    //Types: Adding to combo = 1, Removing from combo = 2, multiply combo = 3, divide combo = 4.

    [Header("Description Box Data")]
    public string programName;
    public string programDescription;

    [HideInInspector] public PlayerLogic player;
    [HideInInspector] public CustomAnimator playerAnimator;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerTargeting playerTargeting;
    [HideInInspector] public ProgramUI programUI;
    [HideInInspector] public ProgramInputManager inputManager;
    [HideInInspector] public ComboBarLogic comboBar;
    
    public virtual void FireProgram(QueueParameter queueParameter)
    {
        
    }

    void Awake()
    {
        FindDependencies();
    }
    
    //In script for any program inheriting this class, run FindDependencies() in Start()
    protected virtual void FindDependencies()
    {
        HitboxTracker.Instance.RegisterHitboxGroup(gameObject, hitboxTimings, hitboxTimings.Length);

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
        comboBar = FindObjectOfType<ComboBarLogic>();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<CustomAnimator>();
        }
    }

    protected virtual void AssignHitboxDamages(int amount)
    {
        foreach(HitboxTiming hitboxTiming in hitboxTimings)
        {
            hitboxTiming.hitbox.damage = amount;
        }
    }

    //When given a direction of -1 or 1 in in direction, this method will change the transform of the hitboxes 
    protected virtual void ChangeTransform(int direction)
    {
        player.ChangeTransform(direction);

        foreach (HitboxTiming timing in hitboxTimings)
        {
            HitBox temp = timing.hitbox;

            if (direction == -1)
            {
                temp.offset.x *= -1;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        HitboxTracker.Instance.UnregisterHitboxGroup(gameObject);
    }
}
