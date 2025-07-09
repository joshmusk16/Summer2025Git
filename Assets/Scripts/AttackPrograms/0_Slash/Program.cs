using UnityEngine;

public class Program : MonoBehaviour
{

    public PlayerLogic player;
    public Animator playerAnimator;
    public AttackUI attackUI;
    public ProgramInputManager inputManager;

    public Sprite[] attackSprites;
    public float[] attackFrames;
    public HitBox[] attackHitboxes;

    //In script for any program inheriting this class, run FindDependencies() in Start()
    public void FindDependencies()
    {
        attackUI = FindObjectOfType<AttackUI>();
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
        foreach (HitBox hitbox in attackHitboxes)
        {
            if (direction == 1)
            {
                hitbox.offset.x = Mathf.Abs(hitbox.offset.x);
            }
            else if (direction == -1)
            {
                hitbox.offset.x = -Mathf.Abs(hitbox.offset.x);
            }
        }
    }
}
