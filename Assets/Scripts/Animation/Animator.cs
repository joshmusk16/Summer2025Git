using UnityEngine;
using System;
using System.Collections.Generic;

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
    public event Action<int> OnFrameChanged;

    private bool isUsingHitbox = false;
    public HitboxTiming[] animHitboxTimings;
    private HashSet<HitBox> activeHitboxes = new HashSet<HitBox>();

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
                UpdateHitboxes();
            }

            SetCurrentFrame();
            OnFrameChanged?.Invoke(currentFrameIndex);
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
                OnFrameChanged?.Invoke(currentFrameIndex);

                if (isUsingHitbox)
                {
                    DeactivateAllHitboxes(); // Reset hitboxes on loop
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

    public void PlayAnimation(Sprite[] sprites, float[] frameTimes, bool loop = false, bool usingHitbox = false, HitboxTiming[] hitboxTimings = null)
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

        if (usingHitbox && hitboxTimings != null)
        {
            CopyHitboxTimings(hitboxTimings);
            isUsingHitbox = true;
        }
        else
        {
            isUsingHitbox = false;
        }

        // Set first frame and start animation
        SetCurrentFrame();
        OnAnimationStart?.Invoke();
        OnFrameChanged?.Invoke(currentFrameIndex);

        isPlaying = true;
    }

    private void CopyHitboxTimings(HitboxTiming[] sourceTimings)
    {
        animHitboxTimings = new HitboxTiming[sourceTimings.Length];
        for (int i = 0; i < sourceTimings.Length; i++)
        {
            animHitboxTimings[i] = new HitboxTiming
            {
                hitbox = sourceTimings[i].hitbox,
                activationFrames = (int[])sourceTimings[i].activationFrames.Clone()
            };
        }
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

        private void UpdateHitboxes()
    {
        if (animHitboxTimings == null) return;

        foreach (HitboxTiming timing in animHitboxTimings)
        {
            bool shouldBeActive = false;
            
            // Check if current frame is in this hitbox's activation frames
            foreach (int frame in timing.activationFrames)
            {
                if (frame == currentFrameIndex)
                {
                    shouldBeActive = true;
                    break;
                }
            }

            // Update hitbox state
            if (shouldBeActive && !activeHitboxes.Contains(timing.hitbox))
            {
                ActivateHitbox(timing.hitbox);
            }
            else if (!shouldBeActive && activeHitboxes.Contains(timing.hitbox))
            {
                DeactivateHitbox(timing.hitbox);
            }
        }
    }

    private void ActivateHitbox(HitBox hitbox)
    {
        if (hitbox != null && !hitbox.isActive)
        {
            Debug.Log("Activated:" + hitbox.name);
            hitbox.isActive = true;
            activeHitboxes.Add(hitbox);
        }
    }
    
    private void DeactivateHitbox(HitBox hitbox)
    {
        if (hitbox != null && hitbox.isActive)
        {
            hitbox.isActive = false;
            activeHitboxes.Remove(hitbox);
        }
    }

    private void DeactivateAllHitboxes()
    {
        foreach (HitBox hitbox in activeHitboxes)
        {
            if (hitbox != null && hitbox.isActive)
            {
                hitbox.isActive = false;
            }
        }
        activeHitboxes.Clear();
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