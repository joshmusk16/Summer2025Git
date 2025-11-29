using UnityEngine;
using System;
using System.Collections.Generic;

public class CustomAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite[] currentSprites;
    public bool isPlaying = false;
    private bool shouldLoop = false;

    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private float totalAnimationTime = 0f;
    private float[] frameTimeThresholds;
    private ProgramType animationType = ProgramType.Other;

    private bool isParameterDriven = false;
    private float[] parameterThresholds;
    private int lastParameterFrameIndex = -1;
    private Func<float> parameterSource;

    public event Action OnAnimationStart;
    public event Action<ProgramType> OnAnimationComplete;
    public event Action<int, ProgramType> OnFrameChanged;

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

        if (isParameterDriven)
        {
            UpdateParameterDrivenAnimation();
        }
        else
        {
            UpdateTimeDrivenAnimation();
        }
    }

    private void UpdateTimeDrivenAnimation()
    {
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
            OnFrameChanged?.Invoke(currentFrameIndex, animationType);
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
                OnFrameChanged?.Invoke(currentFrameIndex, animationType);

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

    private void UpdateParameterDrivenAnimation()
    {
        if (parameterSource == null) return;

        float currentParameter = Mathf.Clamp01(parameterSource());
        int newFrameIndex = GetFrameIndexFromParameter(currentParameter);

        if (newFrameIndex != lastParameterFrameIndex)
        {
            currentFrameIndex = newFrameIndex;
            lastParameterFrameIndex = newFrameIndex;

            if (isUsingHitbox)
            {
                UpdateHitboxes();
            }

            SetCurrentFrame();
            OnFrameChanged?.Invoke(currentFrameIndex, animationType);
        }
        
        if (currentParameter >= 1.0f)
        {
            int lastFrame = currentSprites.Length - 1;
            if (currentFrameIndex != lastFrame)
            {
                currentFrameIndex = lastFrame;
                lastParameterFrameIndex = lastFrame;
                SetCurrentFrame(lastFrame);
                OnFrameChanged?.Invoke(currentFrameIndex, animationType);
            }
            CompleteAnimation();
            return;
        }
    }

    private int GetFrameIndexFromParameter(float parameter)
    {
        // Find which frame should be active based on parameter value
        for (int i = parameterThresholds.Length - 1; i >= 0; i--)
        {
            if (parameter >= parameterThresholds[i])
            {
                return i;
            }
        }
        return 0; // Default to first frame
    }

    public void PlayAnimation(Sprite[] sprites, float[] frameTimes, ProgramType animType, bool loop = false, bool usingHitbox = false,
    HitboxTiming[] hitboxTimings = null)
    {
        if (sprites.Length != frameTimes.Length || sprites.Length == 0)
        {
            return;
        }

        StopAnimation();

        // Set up new animation
        currentSprites = sprites;
        shouldLoop = loop;
        animationType = animType;

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

        SetUpHitboxInfo(usingHitbox, hitboxTimings);
        StartAnimation();
    }

    public void PlayParameterDrivenAnimation(Sprite[] sprites, float[] frameThresholds, ProgramType animType, Func<float> parameter, bool usingHitbox = false, HitboxTiming[] hitboxTimings = null)
    {
        if (sprites.Length != frameThresholds.Length ||
        sprites.Length == 0 || parameter == null) return;

        // Validate that thresholds are in 0-1 range and properly ordered
        for (int i = 0; i < frameThresholds.Length; i++)
        {
            if (frameThresholds[i] < 0f || frameThresholds[i] > 1f) return;
            if (i > 0 && frameThresholds[i] < frameThresholds[i - 1]) return;
        }

        StopAnimation();

        currentSprites = sprites;
        shouldLoop = false;
        isParameterDriven = true;
        parameterSource = parameter;
        animationType = animType;

        parameterThresholds = new float[frameThresholds.Length];
        Array.Copy(frameThresholds, parameterThresholds, frameThresholds.Length);

        currentFrameIndex = 0;
        lastParameterFrameIndex = -1;

        SetUpHitboxInfo(usingHitbox, hitboxTimings);
        StartAnimation();
    }

    private void SetUpHitboxInfo(bool usingHitbox, HitboxTiming[] hitboxTimings)
    {
        if (usingHitbox && hitboxTimings != null)
        {
            animHitboxTimings = new HitboxTiming[hitboxTimings.Length];
            for (int i = 0; i < hitboxTimings.Length; i++)
            {
                animHitboxTimings[i] = new HitboxTiming
                {
                    hitbox = hitboxTimings[i].hitbox,
                    activationFrames = (int[])hitboxTimings[i].activationFrames.Clone()
                };
            }
            isUsingHitbox = true;
        }
        else
        {
            isUsingHitbox = false;
        }
    }

    private void StartAnimation()
    {
        SetCurrentFrame();
        OnAnimationStart?.Invoke();
        OnFrameChanged?.Invoke(currentFrameIndex, animationType);
        isPlaying = true;
    }


    public void StopAnimation()
    {
        isPlaying = false;
        frameTimer = 0f;
        currentFrameIndex = 0;
        lastParameterFrameIndex = -1;
        isParameterDriven = false;
        parameterSource = null;

        if (isUsingHitbox)
        {
            DeactivateAllHitboxes();
        }
    }

    private void CompleteAnimation()
    {
        isPlaying = false;
        OnAnimationComplete?.Invoke(animationType);
    }

    private void SetCurrentFrame()
    {
        SetCurrentFrame(currentFrameIndex);
    }

        private void SetCurrentFrame(int frameIndex)
    {
        if (currentSprites != null && frameIndex < currentSprites.Length && frameIndex >= 0)
        {
            spriteRenderer.sprite = currentSprites[frameIndex];
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
        if (!isPlaying) return 0f;
        
        if (isParameterDriven)
        {
            // For parameter-driven animations, get the current parameter value
            if (parameterSource != null)
            {
                return Mathf.Clamp01(parameterSource());
            }
            return 0f;
        }
        else
        {
            // For time-driven animations, use the frame timer
            if (totalAnimationTime == 0f) return 0f;
            return frameTimer / totalAnimationTime;
        }
    }
}