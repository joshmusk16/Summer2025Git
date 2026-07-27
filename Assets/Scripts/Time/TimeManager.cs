using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static float timeMultiplier;
    public static float normalizedTimeMultiplier = 1f;

    private float currentTimeMultiplier;

    private const float timeMultiplierMax = 5.0f;
    private const float timeMultiplierMin = 1.0f;
    
    private float pauseDuration = 0.15f;
    private float pauseElapsed = 0f;
    private bool unpausing = false;
    private bool pausing = false;

    private ProgramInputManager programInputManager;
    private TimeSpeedUI timeSpeedUI;

    void Start()
    {
        timeMultiplier = timeMultiplierMin;
        currentTimeMultiplier = timeMultiplierMin;

        UpdateTimeSpeedUI();

        programInputManager.OnSlowModeEnter += GraduallyPauseTime;
        programInputManager.OnSlowModeExit += GraduallyUnpauseTime;
    }

    private void FindDependencies()
    {
        if(timeSpeedUI == null) timeSpeedUI = gameObject.GetComponent<TimeSpeedUI>();
        if(programInputManager == null) programInputManager = FindObjectOfType<ProgramInputManager>();
    }

    void Update()
    {
        UpdatePause();
    }

    private void UpdateTimeSpeedUI()
    {
        FindDependencies();
        timeSpeedUI.UpdateGameSpeedUI(timeMultiplier);
    }

    private void StopTime()
    {
        timeMultiplier = 0;
    }

    private void StartTime()
    {
        timeMultiplier = currentTimeMultiplier;
    }

    public void ChangeGameSpeed(int gameSpeedRewardType, float amountToChangeGameSpeed)
    {
        if(gameSpeedRewardType < 1 || gameSpeedRewardType > 2) return;

        switch (gameSpeedRewardType)
        {   
            case 1: 
                IncreaseGameSpeed(amountToChangeGameSpeed);
                break;
            case 2:
                DecreaseGameSpeed(amountToChangeGameSpeed);
                break;
        }
    }

    public void IncreaseGameSpeed(float amount)
    {
        amount = Mathf.Abs((float)System.Math.Round(amount, 2));
        if (currentTimeMultiplier + amount <= timeMultiplierMax)
        {
            currentTimeMultiplier += amount;
            timeMultiplier = currentTimeMultiplier;
            UpdateTimeSpeedUI();
        }
    }

    public void DecreaseGameSpeed(float amount)
    {
        amount = Mathf.Abs((float)System.Math.Round(amount, 2));

        if(currentTimeMultiplier - amount >= timeMultiplierMin)
        {
            currentTimeMultiplier -= amount;
            timeMultiplier = currentTimeMultiplier;
            UpdateTimeSpeedUI();
        }
    }

    public void GraduallyUnpauseTime()
    {
        pausing = false;
        unpausing = true;
        pauseElapsed = 0f;
    }

    public void GraduallyPauseTime()
    {
        Debug.Log("GraduallyPauseTime called");
        unpausing = false;
        pausing = true;
        pauseElapsed = 0f;
    }

    private float EaseInExpo(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    private void UpdatePause()
    {
        if (!unpausing && !pausing) return;

        pauseElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(pauseElapsed / pauseDuration);
        float eased = EaseInExpo(t);
        Debug.Log("Eased is" + eased);

        if (unpausing)
        {
            timeMultiplier = Mathf.Lerp(0f, currentTimeMultiplier, eased);
            normalizedTimeMultiplier = Mathf.Lerp(0f, 1f, eased);
            if (t >= 1f) unpausing = false;
        }
        else if (pausing)
        {
            timeMultiplier = Mathf.Lerp(currentTimeMultiplier, 0f, eased);
            normalizedTimeMultiplier = Mathf.Lerp(1f, 0f, eased);
            normalizedTimeMultiplier = 1 - eased;
            if (t >= 1f) pausing = false;
        }
    }

    private void OnDestroy() 
    {
        programInputManager.OnSlowModeEnter -= GraduallyPauseTime;
        programInputManager.OnSlowModeExit -= GraduallyUnpauseTime;
    }
}