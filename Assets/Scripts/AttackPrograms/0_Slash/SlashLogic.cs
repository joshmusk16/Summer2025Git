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
        player.GetComponent<PlayerLogic>().MouseLeftOrRightChanged += ChangeTransform;
    }

    //update for debugging only
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Slash();
        }
    }

    void Slash()
    {
        playerAnimator.PlayAnimation(slashSprites, slashFrames, false, true, slashHitboxes, 3);
    }

    void ChangeTransform(int direction)
    {
        foreach (HitBox hitbox in slashHitboxes)
        {
            hitbox.offset.x *= -1;
        }
    }

    void OnDestroy()
    {
        player.GetComponent<PlayerLogic>().MouseLeftOrRightChanged -= ChangeTransform;
    }
}
