using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public Animator playerAnimator;

    [Header("Player Idle Info")]
    public Sprite[] idleSprites;
    public float[] idleFrames;

    //start method for debugging, ideally the idle animation is started and stopped manually upon other animations ending

    void Start()
    {
        playerAnimator.OnAnimationComplete += StartIdleAnimation;
        StartIdleAnimation();
    }

    public void StartIdleAnimation()
    {
        playerAnimator.PlayAnimation(idleSprites, idleFrames, true);
    }
    
    private void OnDestroy() {
        playerAnimator.OnAnimationComplete -= StartIdleAnimation;
    }
}
