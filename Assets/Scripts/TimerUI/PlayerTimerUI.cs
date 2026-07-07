using System;
using UnityEngine;

public class PlayerTimerUI : MonoBehaviour
{
    public GameObject playerTimerBar;
    private PlayerTimerLogic playerTimerLogic;
    private SpriteRenderer playerTimerBarSpriteRenderer;

    private bool isAnimating;
    private Vector2 animationDestination;
    private const float ANIMATION_SPEED = 20f;

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
    private const float PARABOLIC_SCALE_DURATION = 0.1f;
    private const float PARABOLIC_STRENGTH = 2f;

    public event Action OnHealthBarAnimationFinish;

    private bool isAnimatingColor;
    public Color activeColor;
    public Color inactiveColor;
    private Color colorAnimationDestination;
    private const float COLOR_ANIMATION_SPEED = 20f;

    void Start()
    {
        playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
        SetIntialDigits();

        if(playerTimerBar != null)
        {
            playerTimerBarSpriteRenderer = playerTimerBar.GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (isAnimating)
        {
            playerTimerBar.transform.localScale = Vector2.Lerp(playerTimerBar.transform.localScale, animationDestination, Time.deltaTime * ANIMATION_SPEED);
            UpdateNumberUI(playerTimerLogic.playerTotalTime);

            if (Mathf.Abs(animationDestination.x - playerTimerBar.transform.localScale.x) < 0.001f)
            {
                playerTimerBar.transform.localScale = animationDestination;
                OnHealthBarAnimationFinish?.Invoke();
                isAnimating = false;
            }
        }

        if (isAnimatingColor)
        {
            playerTimerBarSpriteRenderer.color = Color.Lerp(playerTimerBarSpriteRenderer.color, colorAnimationDestination, Time.deltaTime * COLOR_ANIMATION_SPEED);

            if (ColorDistance(playerTimerBarSpriteRenderer.color, colorAnimationDestination) < 0.001f)
            {
                playerTimerBarSpriteRenderer.color = colorAnimationDestination;
                isAnimatingColor = false;
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

    public void AnimateColorChange(int colorType)
    {
        if(playerTimerBarSpriteRenderer == null) playerTimerBarSpriteRenderer = playerTimerBar.GetComponent<SpriteRenderer>();

        Color currentColor = playerTimerBarSpriteRenderer.color;

        switch (colorType)
        {
            case 1:
                colorAnimationDestination = activeColor;
            break;

            case 2:
                colorAnimationDestination = inactiveColor;
            break;
        }

        if(currentColor == colorAnimationDestination) return;

        isAnimatingColor = true;
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
            hundredsLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, PARABOLIC_SCALE_DURATION, PARABOLIC_STRENGTH);
        }

        if (Mathf.FloorToInt(currentDisplayAmount / 10) % 10 != previousTensDigit)
        {
            previousTensDigit = Mathf.FloorToInt(currentDisplayAmount / 10) % 10;
            tens.UpdateNumber(previousTensDigit);
            tensLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, PARABOLIC_SCALE_DURATION, PARABOLIC_STRENGTH);
        }

        if (Mathf.FloorToInt(currentDisplayAmount % 10) != previousOnesDigit)
        {
            previousOnesDigit = Mathf.FloorToInt(currentDisplayAmount % 10);
            ones.UpdateNumber(previousOnesDigit);
            onesLerpHandler.ParabolicScaleLerp(parabolicAnimationScale, PARABOLIC_SCALE_DURATION, PARABOLIC_STRENGTH);
        }
    }

    private float ColorDistance(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a);
    }
}