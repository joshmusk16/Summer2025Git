using UnityEngine;

public class LerpUIHandler : MonoBehaviour
{
    private bool isLerping = false;
    private bool isScaling = false;

    private Vector2 locationDestination;
    private float locationLerpSpeed;

    private Vector2 scaleDestination;
    private float scaleLerpSpeed;

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

    public void StopLocationLerp()
    {
        isLerping = false;
    }
    
    public void StopScaleLerp()
    {
        isScaling = false;
    }
}
