using System;
using UnityEngine;

public class PlayerTimerUI : MonoBehaviour
{

    public GameObject playerTimerBar;
    private PlayerTimerLogic playerTimerLogic;

    private bool isAnimating;
    private Vector2 animationDestination;
    private const float animationSpeed = 20f;

    private int previousOnesDigit;
    private int previousTensDigit;
    private int previousHundredsDigit;

    public DigitUI ones;
    public DigitUI tens;
    public DigitUI hundreds;

    private LerpUIHandler onesLerpHandler;
    private LerpUIHandler tensLerpHandler;
    private LerpUIHandler hundredsLerpHandler;

    private Vector2 parabolicAnimationScale = new(1.1f, 1.1f);
    private const float parabolicScaleDuration = 0.1f;
    private const float parabolicStrength = 2f;

    public event Action OnHealthBarAnimationFinish;

    void Start()
    {
        playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
        SetIntialDigits();
    }

    void Update()
    {
        if (isAnimating)
        {
            playerTimerBar.transform.localScale = Vector2.Lerp(playerTimerBar.transform.localScale, animationDestination, Time.deltaTime * animationSpeed);
            UpdateNumberUI(playerTimerLogic.playerTotalTime);

            if (Mathf.Abs(animationDestination.x - playerTimerBar.transform.localScale.x) < 0.001f)
            {
                playerTimerBar.transform.localScale = animationDestination;
                OnHealthBarAnimationFinish?.Invoke();
                isAnimating = false;
            }
        }
    }

    public void AnimateHealthChange(float healthChange)
    {
        float scaleX = playerTimerBar.transform.localScale.x;

        if (healthChange == scaleX) return;

        animationDestination = new Vector2(healthChange, 1f);
        isAnimating = true;
    }

    void SetIntialDigits()
    {
        float currentDisplayAmount = playerTimerBar.transform.localScale.x * playerTimerLogic.playerTotalTime;

        previousHundredsDigit = Mathf.FloorToInt(currentDisplayAmount / 100) % 10;
        previousTensDigit = Mathf.FloorToInt(currentDisplayAmount / 10) % 10;
        previousOnesDigit = Mathf.FloorToInt(currentDisplayAmount % 10);

        if (ones != null && tens != null && hundreds != null)
        {
            onesLerpHandler = ones.GetComponent<LerpUIHandler>();
            tensLerpHandler = tens.GetComponent<LerpUIHandler>();
            hundredsLerpHandler = hundreds.GetComponent<LerpUIHandler>();
        }
    }

    void UpdateNumberUI(float totalPlayerTime)
    {
        float currentDisplayAmount = playerTimerBar.transform.localScale.x * totalPlayerTime;

        if (Mathf.FloorToInt(currentDisplayAmount / 100) % 10 != previousHundredsDigit)
        {
            previousHundredsDigit = Mathf.FloorToInt(currentDisplayAmount / 100) % 10;
            hundreds.UpdateNumber(previousHundredsDigit);
            hundredsLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, parabolicScaleDuration, parabolicStrength);
        }

        if (Mathf.FloorToInt(currentDisplayAmount / 10) % 10 != previousTensDigit)
        {
            previousTensDigit = Mathf.FloorToInt(currentDisplayAmount / 10) % 10;
            tens.UpdateNumber(previousTensDigit);
            tensLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, parabolicScaleDuration, parabolicStrength);
        }

        if (Mathf.FloorToInt(currentDisplayAmount % 10) != previousOnesDigit)
        {
            previousOnesDigit = Mathf.FloorToInt(currentDisplayAmount % 10);
            ones.UpdateNumber(previousOnesDigit);
            onesLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, parabolicScaleDuration, parabolicStrength);
        }
    }
}