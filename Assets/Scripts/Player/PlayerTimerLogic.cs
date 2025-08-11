using UnityEngine;

public class PlayerTimerLogic : MonoBehaviour
{

    [Header("Player Timer Data")]
    public float playerTotalTime;
    public float playerCurrentTime;
    [SerializeField] private float timerSpeedMultiplier;
    [SerializeField] private bool playerTimerIsRunning = false;
    private PlayerTimerUI playerHealthUI;

    [SerializeField] private float timerUpdateInterval = 0.5f;
    private float nextUpdateTime;

    void Start()
    {
        playerHealthUI = FindObjectOfType<PlayerTimerUI>();
    }

    void Update()
    {
        if (playerTimerIsRunning == true)
        {
            RunPlayerTimer();
        }

        //Debugging Input
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartRunningTimer();
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
            Debug.Log("Called animateHealthBar at" + playerCurrentTime);
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
    }

    public void AddPlayerTime(HitInfo hitInfo)
    {
        if (hitInfo.damage <= 0) return;

        if (playerCurrentTime + hitInfo.damage <= playerTotalTime)
        {
            playerCurrentTime += hitInfo.damage;
        }
        else
        {
            playerCurrentTime = playerTotalTime;
        }

        AnimateHealthBar();
    }

    public void RemovePlayerHealth(HitInfo hitInfo)
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

    public void AddPlayerTotalHealth(int health)
    {
        if (health <= 0) return;

        playerTotalTime += health;

        AnimateHealthBar();
    }

    public void RemovePlayerTotalHealth(int health)
    {
        if (health <= 0) return;

        playerTotalTime -= health;

        if (playerCurrentTime > playerTotalTime)
        {
            playerCurrentTime = playerTotalTime;
        }

        AnimateHealthBar();
    }

    private void AnimateHealthBar()
    {
        nextUpdateTime = playerCurrentTime - timerUpdateInterval;
        playerHealthUI.AnimateHealthChange(playerCurrentTime / playerTotalTime);
    }

}
