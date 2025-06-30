using UnityEngine;

public class SlashLogic : MonoBehaviour
{
    private GameObject player;
    private Animator playerAnimator;

    public Sprite[] slashSprites;
    public float[] slashFrames;

    public HitBox[] slashHitboxes;

    void Start()
    {
        player = FindObjectOfType<PlayerLogic>().gameObject;
        playerAnimator = player.GetComponent<Animator>();
    }

    //update for debugging only
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Slash();
        }
    }

    void Slash()
    {
        playerAnimator.PlayAnimation(slashSprites, slashFrames, false, true, slashHitboxes, 3);
    }
}
