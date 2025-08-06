using System;
using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{

    public GameObject playerHealthBar;

    private bool isAnimating;
    private Vector2 animationDestination;
    private const float animationSpeed = 10f;

    public event Action OnHealthBarAnimationFinish;

    void Update()
    {
        if (isAnimating)
        {
            playerHealthBar.transform.localScale = Vector2.Lerp(playerHealthBar.transform.localScale, animationDestination, Time.deltaTime * animationSpeed);

            if (Mathf.Abs(animationDestination.x - playerHealthBar.transform.localScale.x) < 0.0001f)
            {
                playerHealthBar.transform.localScale = animationDestination;
                OnHealthBarAnimationFinish?.Invoke();
                isAnimating = false;
            }
        }
    }

    public void AnimateHealthChange(float healthChange)
    {
        float scaleX = playerHealthBar.transform.localScale.x;

        if (healthChange == scaleX) return;

        animationDestination = new Vector2(healthChange, 1f);
        isAnimating = true;
    }
}
