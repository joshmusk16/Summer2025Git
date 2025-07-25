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

    public PlayerLogic player;
    public Animator playerAnimator;
    public ProgramUI programUI;
    public ProgramInputManager inputManager;

    public Sprite[] attackSprites;
    public float[] attackFrames;
    public HitboxTiming[] hitboxTimings;


    //In script for any program inheriting this class, run FindDependencies() in Start()
    public void FindDependencies()
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
        inputManager = FindObjectOfType<ProgramInputManager>();

        if (player != null)
        {
            playerAnimator = player.gameObject.GetComponent<Animator>();
        }
    }

    //When given a direction of -1 or 1 in in direction, this method will change the transform of the hitboxes 
    //appropriately. This is intended to be subscribed to MouseLeftOrRightOfPlayer() in the PlayerHandler script
    //See SlashLogic Script for reference
    public void ChangeTransform(int direction)
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
