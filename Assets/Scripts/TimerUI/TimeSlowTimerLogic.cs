using UnityEngine;

public class TimeSlowTimerLogic : MonoBehaviour
{
    [Header("Time Slow Timer Data")]
    public float timeSlowTotalTime;
    public float timeSlowCurrentTime;

    [SerializeField] private float timerSpeedDecreaseMultiplier;
    [SerializeField] private float timerSpeedIncreaseMultiplier;

    [SerializeField] private bool timeSlowTimerIsDecreasing = false;
    [SerializeField] private bool timeSlowTimerIsIncreasing = false;
    [SerializeField] private bool rechargeDelayRunning = false;

    public float rechargeDelayTotalTime;
    public float rechargeDelayCurrentTime;

    private ProgramInputManager inputManager;
    public GameObject timeSlowBar;

    void Start()
    {
        if (timeSlowCurrentTime != timeSlowTotalTime)
        {
            timeSlowCurrentTime = timeSlowTotalTime;
        }

        inputManager = FindObjectOfType<ProgramInputManager>();

        if (inputManager != null)
        {
            inputManager.OnSlowModeEnter += StartDecreasingTimeSlowTimer;
            inputManager.OnSlowModeEnter += StopAndResetAllTimers;
            inputManager.OnSlowModeExit += StopDecreasingTimeSlowTimer;
            inputManager.OnSlowModeExit += StartRechargeDelay;
        }
    }

    void StartDecreasingTimeSlowTimer()
    {
        timeSlowTimerIsDecreasing = true;
    }

    void StopDecreasingTimeSlowTimer()
    {
        timeSlowTimerIsDecreasing = false;
    }

    void StartIncreasingTimeSlowTimer()
    {
        timeSlowTimerIsIncreasing = true;
    }

    void StopIncreasingTimeSlowTimer()
    {
        timeSlowTimerIsIncreasing = false;
    }

    void StartRechargeDelay()
    {
        rechargeDelayRunning = true;
    }

    void DecreaseTimeSlowTimer()
    {
        if (timeSlowCurrentTime > 0)
        {
            timeSlowCurrentTime -= Time.deltaTime * timerSpeedDecreaseMultiplier;
            AnimateTimeSlowBar();
        }

        if (timeSlowCurrentTime < 0)
        {
            timeSlowCurrentTime = 0;
            StopDecreasingTimeSlowTimer();
            AnimateTimeSlowBar();
            inputManager.ForceExitSlowMode();
        }
    }

    void RunRechargeDelay()
    {
        if (rechargeDelayCurrentTime < rechargeDelayTotalTime)
        {
            rechargeDelayCurrentTime += Time.deltaTime;
        }

        if (rechargeDelayCurrentTime > rechargeDelayTotalTime)
        {
            StartIncreasingTimeSlowTimer();
            rechargeDelayRunning = false;
            rechargeDelayCurrentTime = 0;
        }
    }

    void IncreaseTimeSlowTimer()
    {
        if (timeSlowCurrentTime < timeSlowTotalTime)
        {
            timeSlowCurrentTime += Time.deltaTime * timerSpeedIncreaseMultiplier;
            AnimateTimeSlowBar();
        }

        if (timeSlowCurrentTime >= timeSlowTotalTime)
        {
            timeSlowCurrentTime = timeSlowTotalTime;
            StopIncreasingTimeSlowTimer();
            AnimateTimeSlowBar();
        }
    }

    void Update()
    {
        if (timeSlowTimerIsDecreasing)
        {
            DecreaseTimeSlowTimer();
        }

        if (timeSlowTimerIsIncreasing)
        {
            IncreaseTimeSlowTimer();
        }

        if (rechargeDelayRunning)
        {
            RunRechargeDelay();
        }
    }

    void AnimateTimeSlowBar()
    {
        float ratio = timeSlowCurrentTime / timeSlowTotalTime;
        timeSlowBar.transform.localScale = new(ratio, 1);
    }

    void StopAndResetAllTimers()
    {
        rechargeDelayCurrentTime = 0;
        rechargeDelayRunning = false;
        StopIncreasingTimeSlowTimer();
    }

}
