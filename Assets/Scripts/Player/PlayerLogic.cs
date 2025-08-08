using System;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    private CustomAnimator playerAnimator;
    public MouseTracker mouseTracker;

    [Header("Player Health Data")]
    [SerializeField] private float playerTotalHealth;
    [SerializeField] private float playerCurrentHealth;
    private PlayerHealthUI playerHealthUI;

    private HurtBox playerHurtBox;

    [Header("Player Idle Info")]
    public Sprite[] idleSprites;
    public float[] idleFrames;

    public event Action<int> MouseLeftOrRightChanged;
    public int currentMouseLeftOrRight = 1;

    //start method for debugging, ideally the idle animation is started and stopped manually upon other animations ending

    void Start()
    {
        playerHealthUI = FindObjectOfType<PlayerHealthUI>();
        playerHurtBox = gameObject.GetComponent<HurtBox>();
        playerAnimator = gameObject.GetComponent<CustomAnimator>();
        
        if (playerHurtBox != null)
        {
            playerHurtBox.OnHit += RemovePlayerHealth;
        }

        playerAnimator.OnAnimationComplete += StartIdleAnimation;
        StartIdleAnimation(ProgramType.Other);
    }

    void Update()
    {
        MouseLeftOrRightOfPlayer();
    }

    #region Player Health Methods

    public void AddPlayerHealth(HitInfo hitInfo)
    {
        if (hitInfo.damage <= 0) return;
        
        if (playerCurrentHealth + hitInfo.damage <= playerTotalHealth)
        {
            playerCurrentHealth += hitInfo.damage;
        }
        else
        {
            playerCurrentHealth = playerTotalHealth;
        }

        AnimateHealthBar();
    }

    public void RemovePlayerHealth(HitInfo hitInfo)
    {
        if (hitInfo.damage <= 0) return;

        if (playerCurrentHealth - hitInfo.damage > 0)
        {
            playerCurrentHealth -= hitInfo.damage;
        }
        else
        {
            playerCurrentHealth = 0;
        }

        AnimateHealthBar();
    }

    public void AddPlayerTotalHealth(int health)
    {
        if (health <= 0) return;

        playerTotalHealth += health;

        AnimateHealthBar();
    }

    public void RemovePlayerTotalHealth(int health)
    {
        if (health <= 0) return;

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

    //Need to test hurtbox methods with 0_LockLogic still
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
    }
}
