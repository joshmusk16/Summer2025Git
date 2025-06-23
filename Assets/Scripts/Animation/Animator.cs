using UnityEngine;
using System;

public class Animator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite[] currentSprites;
    private float[] currentTimes;
    private bool isPlaying = false;
    private bool shouldLoop = false;

    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private float currentFrameDuration = 0f;

    public event Action OnAnimationStart;
    public event Action OnAnimationComplete;

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

        // Check if current frame duration has elapsed
        if (frameTimer >= currentFrameDuration)
        {
            // Move to next frame
            currentFrameIndex++;
            frameTimer = 0f;

            // Check if we've reached the end of the animation
            if (currentFrameIndex >= currentSprites.Length)
            {
                if (shouldLoop)
                {
                    // Loop back to beginning
                    currentFrameIndex = 0;
                    SetCurrentFrame();
                }
                else
                {
                    // Animation complete
                    CompleteAnimation();
                    return;
                }
            }
            else
            {
                SetCurrentFrame();
            }
        }
    }

    public void PlayAnimation(Sprite[] sprites, float[] frameTimes, bool loop = false)
    {
        if (sprites.Length != frameTimes.Length || sprites.Length == 0)
        {
            return;
        }

        StopAnimation();

        // Set up new animation
        currentSprites = sprites;
        currentTimes = frameTimes;
        shouldLoop = loop;

        // Reset animation state
        currentFrameIndex = 0;
        frameTimer = 0f;
        isPlaying = true;

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
            currentFrameDuration = currentTimes[currentFrameIndex];
        }
    }
}
