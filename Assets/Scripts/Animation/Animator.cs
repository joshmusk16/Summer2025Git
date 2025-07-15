using UnityEngine;
using System;

public class Animator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite[] currentSprites;
    public bool isPlaying = false;
    private bool shouldLoop = false;

    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private float totalAnimationTime = 0f;
    private float[] frameTimeThresholds;

    public event Action OnAnimationStart;
    public event Action OnAnimationComplete;

    private bool isUsingHitbox = false;
    private HitBox[] animHitboxes;
    private int activeHitboxFrame;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isPlaying || currentSprites == null) return;

        // Update frame timer
        frameTimer += Time.deltaTime;

        // Check if we need to change frames based on time thresholds
        if (currentFrameIndex < frameTimeThresholds.Length - 1 && frameTimer >= frameTimeThresholds[currentFrameIndex + 1])
        {
            // Move to next frame
            currentFrameIndex++;
            
            if (isUsingHitbox)
            {
                if (animHitboxes != null && currentFrameIndex == activeHitboxFrame)
                {
                    ActivateHitboxes();
                }
                else if (currentFrameIndex != activeHitboxFrame)
                {
                    DeactivateHitboxes();
                }   
            }

            SetCurrentFrame();
        }

        // Check if animation is complete
        if (frameTimer >= totalAnimationTime)
        {
            if (shouldLoop)
            {
                // Loop back to beginning
                frameTimer = 0f;
                currentFrameIndex = 0;
                SetCurrentFrame();
                if (isUsingHitbox)
                {
                    DeactivateHitboxes(); // Reset hitboxes on loop
                }
            }
            else
            {
                // Animation complete
                CompleteAnimation();
                return;
            }
        }
    }

    public void PlayAnimation(Sprite[] sprites, float[] frameTimes, bool loop = false, bool usingHitbox = false, HitBox[] hitboxes = null, int hitboxFrame = 0)
    {
        if (sprites.Length != frameTimes.Length || sprites.Length == 0)
        {
            return;
        }

        StopAnimation();

        // Set up new animation
        currentSprites = sprites;
        shouldLoop = loop;

        // Calculate total animation time
        totalAnimationTime = 0f;
        for (int i = 0; i < frameTimes.Length; i++)
        {
            totalAnimationTime += frameTimes[i];
        }

        // Calculate frame time thresholds
        frameTimeThresholds = new float[frameTimes.Length];
        float cumulativeTime = 0f;
        for (int i = 0; i < frameTimes.Length; i++)
        {
            frameTimeThresholds[i] = cumulativeTime;
            cumulativeTime += frameTimes[i];
        }

        // Reset animation state
        currentFrameIndex = 0;
        frameTimer = 0f;
        isPlaying = true;

        if (usingHitbox)
        {
            isUsingHitbox = true;
            animHitboxes = hitboxes;
            activeHitboxFrame = hitboxFrame;
        }
        else
        {
            isUsingHitbox = false;
        }

        // Set first frame and start animation
        SetCurrentFrame();
        OnAnimationStart?.Invoke();
    }

    public void StopAnimation()
    {
        isPlaying = false;
        frameTimer = 0f;
        currentFrameIndex = 0;
    }

    private void CompleteAnimation()
    {
        isPlaying = false;
        OnAnimationComplete?.Invoke();
    }

    private void SetCurrentFrame()
    {
        if (currentSprites != null && currentFrameIndex < currentSprites.Length)
        {
            spriteRenderer.sprite = currentSprites[currentFrameIndex];
        }
    }

    private void ActivateHitboxes()
    {
        if (animHitboxes != null)
        {
            foreach (HitBox hitbox in animHitboxes)
            {
                if (hitbox.isActive == false)
                {
                    hitbox.isActive = true;
                }
            }
        }
    }
    
    private void DeactivateHitboxes()
    {
        if (animHitboxes != null)
        {
            foreach (HitBox hitbox in animHitboxes)
            {
                if (hitbox.isActive == true)
                {
                    hitbox.isActive = false;
                }
            }   
        }
    }

    // Helper method to get remaining animation time
    public float GetRemainingAnimationTime()
    {
        if (!isPlaying) return 0f;
        return totalAnimationTime - frameTimer;
    }

    // Helper method to get animation progress (0 to 1)
    public float GetAnimationProgress()
    {
        if (!isPlaying || totalAnimationTime == 0f) return 0f;
        return frameTimer / totalAnimationTime;
    }
}