using System;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public Animator playerAnimator;
    public MouseTracker mouseTracker;

    private float playerTotalHealth;
    private float playerCurrentHealth;
    private PlayerHealthUI playerHealthUI;

    [Header("Player Idle Info")]
    public Sprite[] idleSprites;
    public float[] idleFrames;

    public event Action<int> MouseLeftOrRightChanged;
    public int currentMouseLeftOrRight = 1;


    //start method for debugging, ideally the idle animation is started and stopped manually upon other animations ending

    void Start()
    {
        playerHealthUI = FindObjectOfType<PlayerHealthUI>();

        playerAnimator.OnAnimationComplete += StartIdleAnimation;
        StartIdleAnimation(ProgramType.Other);
    }

    void Update()
    {
        MouseLeftOrRightOfPlayer();
    }

    #region Player Health Methods

    public void AddPlayerHealth(int health)
    {
        if (playerCurrentHealth + health <= playerTotalHealth)
        {
            playerCurrentHealth += health;
        }
        else
        {
            playerCurrentHealth = playerTotalHealth;
        }

        AnimateHealthBar();
    }

    public void RemovePlayerHealth(int health)
    {
        if (playerCurrentHealth - health > 0)
        {
            playerCurrentHealth -= health;
        }
        else
        {
            playerCurrentHealth = 0;
        }

        AnimateHealthBar();
    }

    public void AddPlayerTotalHealth(int health)
    {
        playerTotalHealth += health;

        AnimateHealthBar();
    }

    public void RemovePlayerTotalHealth(int health)
    {
        playerTotalHealth -= health;

        if (playerCurrentHealth > playerTotalHealth)
        {
            playerCurrentHealth = playerTotalHealth;
        }

        AnimateHealthBar();
    }

    private void AnimateHealthBar()
    {
        playerHealthUI.AnimateHealthChange(playerCurrentHealth / playerTotalHealth);
    }

    #endregion

    public void StartIdleAnimation(ProgramType animType)
    {
        playerAnimator.PlayAnimation(idleSprites, idleFrames, animType, true);
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

    private void OnDestroy()
    {
        playerAnimator.OnAnimationComplete -= StartIdleAnimation;
    }
}
