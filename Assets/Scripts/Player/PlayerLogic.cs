using System;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public Animator playerAnimator;
    public MouseTracker mouseTracker;

    [Header("Player Idle Info")]
    public Sprite[] idleSprites;
    public float[] idleFrames;

    public event Action<int> MouseLeftOrRightChanged;
    public int currentMouseLeftOrRight = 1;


    //start method for debugging, ideally the idle animation is started and stopped manually upon other animations ending

    void Start()
    {
        playerAnimator.OnAnimationComplete += StartIdleAnimation;
        StartIdleAnimation();
    }

    void Update()
    {
        MouseLeftOrRightOfPlayer();
    }

    public void StartIdleAnimation()
    {
        playerAnimator.PlayAnimation(idleSprites, idleFrames, true);
    }

    private void OnDestroy()
    {
        playerAnimator.OnAnimationComplete -= StartIdleAnimation;
    }

    //returns -1 for mouse left of the player and 1 for mouse right of the player
    public void MouseLeftOrRightOfPlayer()
    {
        if (mouseTracker.worldPosition.x <= gameObject.transform.position.x &&
        currentMouseLeftOrRight == 1)
        {
            transform.localScale *= new Vector2(-1, 1f);
            MouseLeftOrRightChanged?.Invoke(-1);
            currentMouseLeftOrRight = -1;
        }
        else if (mouseTracker.worldPosition.x > gameObject.transform.position.x &&
        currentMouseLeftOrRight == -1)
        {
            transform.localScale *= new Vector2(-1, 1f);
            MouseLeftOrRightChanged?.Invoke(1);
            currentMouseLeftOrRight = 1;
        }
    }
}
