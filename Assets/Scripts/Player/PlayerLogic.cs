using System;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    private CustomAnimator playerAnimator;
    private PlayerTimerLogic playerTimerLogic;
    public MouseTracker mouseTracker;

    private HurtBox playerHurtBox;

    [Header("Player Idle Info")]
    public Sprite[] idleSprites;
    public float[] idleFrames;

    public event Action<int> MouseLeftOrRightChanged;
    public int currentMouseLeftOrRight = 1;

    //start method for debugging, ideally the idle animation is started and stopped manually upon other animations ending

    void Start()
    {
        playerHurtBox = gameObject.GetComponent<HurtBox>();
        playerAnimator = gameObject.GetComponent<CustomAnimator>();
        playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
        
        if (playerHurtBox != null)
        {
            playerHurtBox.OnHit += playerTimerLogic.RemovePlayerHealth;
        }

        playerAnimator.OnAnimationComplete += StartIdleAnimation;
        StartIdleAnimation(ProgramType.Other);
    }

    void Update()
    {
        MouseLeftOrRightOfPlayer();
    }

    #region Player HurtBox Methods

    public void EnablePlayerHitbox()
    {
        if (playerHurtBox == null || playerHurtBox.isActive == true) return;
        playerHurtBox.isActive = true;
    }

    public void DisablePlayerHitbox()
    {
        if (playerHurtBox == null || playerHurtBox.isActive == false) return;
        playerHurtBox.isActive = false;
    }

    #endregion

    public void StartIdleAnimation(ProgramType animType)
    {
        playerAnimator.PlayAnimation(idleSprites, idleFrames, animType, true);
    }

    //returns -1 for mouse left of the player and 1 for mouse right of the player
    public void MouseLeftOrRightOfPlayer()
    {
        if (mouseTracker.GetWorldMousePosition().x <= gameObject.transform.position.x &&
        currentMouseLeftOrRight == 1)
        {
            transform.localScale *= new Vector2(-1, 1f);
            MouseLeftOrRightChanged?.Invoke(-1);
            currentMouseLeftOrRight = -1;
        }
        else if (mouseTracker.GetWorldMousePosition().x > gameObject.transform.position.x &&
        currentMouseLeftOrRight == -1)
        {
            transform.localScale *= new Vector2(-1, 1f);
            MouseLeftOrRightChanged?.Invoke(1);
            currentMouseLeftOrRight = 1;
        }
    }

    private void OnDestroy()
    {
        playerAnimator.OnAnimationComplete -= StartIdleAnimation;
        playerHurtBox.OnHit -= playerTimerLogic.RemovePlayerHealth;
    }
}
