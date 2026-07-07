using UnityEngine;

public class PlayerTimerLogic : MonoBehaviour
{
    [Header("Player Timer Data")]
    public float playerTotalTime;
    public float playerCurrentTime;
    [SerializeField] private float timerSpeedMultiplier;
    private const float TIMER_SPEED_SLOWMODE_DIFFERENCE = 1.5f;

    [SerializeField] private bool playerTimerIsRunning = false;
    
    private PlayerTimerUI playerHealthUI;
    private ProgramInputManager programInputManager;

    [SerializeField] private float timerUpdateInterval = 1f;
    private float nextUpdateTime;

    void Awake()
    {
        playerHealthUI = FindObjectOfType<PlayerTimerUI>();
        programInputManager = FindObjectOfType<ProgramInputManager>();

        if(programInputManager != null)
        {
            programInputManager.OnSlowModeEnter += DecreaseTimerSpeedMultiplier;
            programInputManager.OnSlowModeExit += IncreaseTimerSpeedMultiplier;
        }

        //StartRunningTimer();
    }

    void Update()
    {
        if (playerTimerIsRunning == true)
        {
            RunPlayerTimer();
        }

        //DEBUGGING INPUTS
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartRunningTimer();
        }

          if (Input.GetKeyDown(KeyCode.H))
        {
            StopRunningTimer();
        }
    }

    public void RunPlayerTimer()
    {
        if (playerCurrentTime > 0)
        {
            playerCurrentTime -= Time.deltaTime * timerSpeedMultiplier;
        }

        if (playerCurrentTime < nextUpdateTime)
        {
            AnimateHealthBar();
        }

        if (playerCurrentTime < 0)
        {
            AnimateHealthBar();
            playerCurrentTime = 0;
            playerTimerIsRunning = false;
        }
    }

    public void StartRunningTimer()
    {
        nextUpdateTime = playerCurrentTime - timerUpdateInterval;
        playerTimerIsRunning = true;
        playerHealthUI.AnimateColorChange(1);
    }

    public void StopRunningTimer()
    {
        playerTimerIsRunning = false;
        playerHealthUI.AnimateColorChange(2);
    }

    public void AddPlayerTime(int amount)
    {
        if (amount <= 0) return;

        if (playerCurrentTime + amount <= playerTotalTime)
        {
            playerCurrentTime += amount;
        }
        else
        {
            playerCurrentTime = playerTotalTime;
        }

        AnimateHealthBar();
    }

    public void RemovePlayerTimeOnHit(HitInfo hitInfo)
    {
        if (hitInfo.damage <= 0) return;

        if (playerCurrentTime - hitInfo.damage > 0)
        {
            playerCurrentTime -= hitInfo.damage;
        }
        else
        {
            playerCurrentTime = 0;
        }

        AnimateHealthBar();
    }

    public void RemovePlayerTime(int amount)
    {
        if (amount <= 0) return;

        if (playerCurrentTime - amount > 0)
        {
            playerCurrentTime -= amount;
        }
        else
        {
            playerCurrentTime = 0;
        }

        AnimateHealthBar();
    }

    public void AddPlayerTotalTime(int health)
    {
        if (health <= 0) return;

        playerTotalTime += health;

        AnimateHealthBar();
    }

    public void RemovePlayerTotalTime(int health)
    {
        if (health <= 0) return;

        playerTotalTime -= health;

        if (playerCurrentTime > playerTotalTime)
        {
            playerCurrentTime = playerTotalTime;
        }

        AnimateHealthBar();
    }

    public void ChangeTimerBar(int type, float amount)
    {
        if(type < 1 || type > 2) return;

        switch (type)
        {
            case 1: 
                AddPlayerTime((int)amount);
                break;
            case 2:
                RemovePlayerTime((int)amount);
                break;
        }
    }

    public void IncreaseTimerSpeedMultiplier()
    {
        timerSpeedMultiplier += TIMER_SPEED_SLOWMODE_DIFFERENCE;
    }

    public void DecreaseTimerSpeedMultiplier()
    {
        timerSpeedMultiplier -= TIMER_SPEED_SLOWMODE_DIFFERENCE;
    }

    private void AnimateHealthBar()
    {
        nextUpdateTime = playerCurrentTime - timerUpdateInterval;
        playerHealthUI.AnimateHealthChange(playerCurrentTime / playerTotalTime);
    }

    void OnDestroy()
    {
        programInputManager.OnSlowModeEnter -= DecreaseTimerSpeedMultiplier;
        programInputManager.OnSlowModeExit -= IncreaseTimerSpeedMultiplier;
    }

}
