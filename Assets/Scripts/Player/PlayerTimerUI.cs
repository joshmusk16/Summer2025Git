using System;
using UnityEngine;

public class PlayerTimerUI : MonoBehaviour
{

    public GameObject playerTimerBar;
    private PlayerTimerLogic playerTimerLogic;

    private bool isAnimating;
    private Vector2 animationDestination;
    private const float animationSpeed = 20f;

    public DigitUI ones;
    public DigitUI tens;
    public DigitUI hundreds;

    public event Action OnHealthBarAnimationFinish;

    void Start()
    {
        playerTimerLogic = FindObjectOfType<PlayerTimerLogic>();
    }

    void Update()
    {
        if (isAnimating)
        {
            playerTimerBar.transform.localScale = Vector2.Lerp(playerTimerBar.transform.localScale, animationDestination, Time.deltaTime * animationSpeed);
            UpdateNumberUI(playerTimerLogic.playerTotalTime);

            if (Mathf.Abs(animationDestination.x - playerTimerBar.transform.localScale.x) < 0.0001f)
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

    void UpdateNumberUI(float totalPlayerTime)
    {
        float currentDisplayAmount = playerTimerBar.transform.localScale.x * totalPlayerTime;

        hundreds.UpdateNumber(Mathf.FloorToInt(currentDisplayAmount / 100) % 10);
        tens.UpdateNumber(Mathf.FloorToInt(currentDisplayAmount / 10) % 10);
        ones.UpdateNumber(Mathf.FloorToInt(currentDisplayAmount % 10));
    }
}
 