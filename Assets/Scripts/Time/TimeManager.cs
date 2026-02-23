using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static float timeMultiplier = 1;
    
    public float pauseDuration = 0.05f;
    public float pauseElapsed = 0f;
    private bool unpausing = false;
    private bool pausing = false;

    public ProgramInputManager programInputManager;

    void Start()
    {
        programInputManager.OnSlowModeEnter += GraduallyPauseTime;
        programInputManager.OnSlowModeExit += GraduallyUnpauseTime;
    }

    void Update()
    {
        UpdatePause();
    }

    public void StopTime()
    {
        timeMultiplier = 0;
    }

    public void StartTime()
    {
        timeMultiplier = 1;
    }

    public void GraduallyUnpauseTime()
    {
        pausing = false;
        unpausing = true;
        pauseElapsed = 0f;
    }

    public void GraduallyPauseTime()
    {
        unpausing = false;
        pausing = true;
        pauseElapsed = 0f;
    }

    private float EaseInExpo(float x)
    {
        return x == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
    }

    private void UpdatePause()
    {
        if (!unpausing && !pausing) return;

        pauseElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(pauseElapsed / pauseDuration);

        if (unpausing)
        {
            timeMultiplier = EaseInExpo(t);
            if (timeMultiplier >= 1f) unpausing = false;
        }
        else if (pausing)
        {
            timeMultiplier = 1f - EaseInExpo(t);
            if (timeMultiplier <= 0f) pausing = false;
        }
    }

    private void OnDestroy() 
    {
        programInputManager.OnSlowModeEnter -= GraduallyPauseTime;
        programInputManager.OnSlowModeExit -= GraduallyUnpauseTime;
    }
}