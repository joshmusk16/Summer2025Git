using UnityEngine;

public class LerpUIHandler : MonoBehaviour
{
    private bool isLerping = false;
    private bool isScaling = false;
    private bool isParabolicLerping = false;

    private Vector2 locationDestination;
    private float locationLerpSpeed;

    private Vector2 scaleDestination;
    private float scaleLerpSpeed;

    private Vector2 parabolicStartScale;
    private Vector2 parabolicPeakScale;
    private float parabolicDuration;
    private float parabolicElapsedTime;
    private float parabolicExponentialStrength = 2.0f;

    void Update()
    {
        if (isLerping)
        {
            gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, locationDestination, Time.deltaTime * locationLerpSpeed);

            if (Vector2.Distance(transform.position, locationDestination) < 0.01f)
            {
                transform.position = locationDestination;
                isLerping = false;
            }
        }

        if (isScaling)
        {
            gameObject.transform.localScale = Vector2.Lerp(gameObject.transform.localScale, scaleDestination, Time.deltaTime * scaleLerpSpeed);

            if (Vector2.Distance(transform.localScale, scaleDestination) < 0.01f)
            {
                transform.localScale = scaleDestination;
                isScaling = false;
            }
        }

        if (isParabolicLerping)
        {
            parabolicElapsedTime += Time.deltaTime;
            float t = parabolicElapsedTime / parabolicDuration;

            if (t >= 1.0f)
            {
                // Animation complete - set to original scale
                transform.localScale = parabolicStartScale;
                isParabolicLerping = false;
            }
            else
            {
                // Apply parabolic interpolation
                Vector2 currentScale = ParabolicExponentialLerp(parabolicStartScale, parabolicPeakScale, t, parabolicExponentialStrength);
                transform.localScale = currentScale;
            }
        }
    }

    public void LocationLerp(Vector2 destination, float speed)
    {
        if ((Vector2)transform.position != destination)
        {
            isLerping = true;
            locationDestination = destination;
            locationLerpSpeed = speed;
        }
    }

    public void ScaleLerp(Vector2 desiredScale, float speed)
    {
        if ((Vector2)transform.localScale != desiredScale)
        {
            isScaling = true;
            scaleDestination = desiredScale;
            scaleLerpSpeed = speed;
        }
    }

    public void ParabolicScaleLerp(Vector2 peakScale, float duration, float exponentialStrength)
    {
        if (duration <= 0) return;

        isParabolicLerping = true;
        parabolicStartScale = transform.localScale;
        parabolicPeakScale = peakScale;
        parabolicDuration = duration;
        parabolicElapsedTime = 0f;
        parabolicExponentialStrength = exponentialStrength;
    }

    public void StopLocationLerp()
    {
        isLerping = false;
    }

    public void StopScaleLerp()
    {
        isScaling = false;
    }

    public void StopParabolicLerp()
    {
        isParabolicLerping = false;
    }
    

    private Vector2 ParabolicExponentialLerp(Vector2 startValue, Vector2 peakValue, float t, float exponentialStrength)
    {
        t = Mathf.Clamp01(t);

        // Parabolic curve (peaks at t = 0.5)
        float parabolicCurve = 4.0f * t * (1.0f - t);

        // Apply exponential transformation
        float exponentialCurve = Mathf.Pow(parabolicCurve, exponentialStrength);

        // Lerp between start and peak values
        return Vector2.Lerp(startValue, peakValue, exponentialCurve);
    }

}
