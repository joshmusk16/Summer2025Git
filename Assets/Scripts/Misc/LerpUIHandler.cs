using UnityEngine;

public class LerpUIHandler : MonoBehaviour
{
    private bool isLerping = false;
    private bool isScaling = false;
    private bool isParabolicLerping = false;
    private bool isElasticLerping = false;

    private Vector2 locationDestination;
    private float locationLerpSpeed;

    private Vector2 scaleDestination;
    private float scaleLerpSpeed;

    private Vector2 parabolicStartScale;
    private Vector2 parabolicPeakScale;
    private float parabolicDuration;
    private float parabolicElapsedTime;
    private float parabolicExponentialStrength = 2.0f;

    private Vector2 elasticStartScale;
    private Vector2 elasticEndScale;
    private float elasticDuration;
    private float elasticElapsedTime;

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

        if (isElasticLerping)
        {
            elasticElapsedTime += Time.deltaTime;
            float t = elasticElapsedTime / elasticDuration;

            if (t >= 1.0f)
            {
                // Animation complete - set to exact destination
                transform.localScale = elasticEndScale;
                isElasticLerping = false;
            }
            else
            {
                // Apply elastic easing
                float easedT = EaseOutElastic(t);
                transform.localScale = Vector2.Lerp(elasticStartScale, elasticEndScale, easedT);
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

        // Stop other scale lerping if active
        isElasticLerping = false;
        isParabolicLerping = false;
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

        // Stop other scale lerping if active
        isElasticLerping = false;
        isScaling = false;
    }

    public void ElasticScaleLerp(Vector2 startScale, Vector2 endScale, float duration)
    {
        if (duration <= 0) return;
        if ((Vector2)transform.localScale == endScale) return;

        isElasticLerping = true;
        elasticStartScale = startScale;
        elasticEndScale = endScale;
        elasticDuration = duration;
        elasticElapsedTime = 0f;

        // Stop other scale lerping if active
        isScaling = false;
        isParabolicLerping = false;
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

    public void StopElasticLerp()
    {
        isElasticLerping = false;
    }

    public void StopAllLerps()
    {
        isLerping = false;
        isScaling = false;
        isParabolicLerping = false;
        isElasticLerping = false;
    }

    //Algorithms for for complex lerp function

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
    
    private float EaseOutElastic(float x)
    {
        const float c4 = 2f * Mathf.PI / 2f;
        
        return x == 0f ? 0f : 
               x == 1f ? 1f : 
               Mathf.Pow(2f, -15f * x) * Mathf.Sin((x * 15f - 0.5f) * c4) + 1f;
    }

}
