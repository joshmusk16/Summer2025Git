using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool isMoving = false;
    private Vector2 locationDestination;
    private Vector2 startPosition;
    private float locationLerpSpeed;
    public event Action OnLocationLerpFinish;

    void Update()
    {
        if (isMoving)
        {
            gameObject.transform.position = Vector2.Lerp(gameObject.transform.position, locationDestination, Time.deltaTime * locationLerpSpeed);

            if (Vector2.Distance(transform.position, locationDestination) < 0.05f)
            {
                transform.position = locationDestination;
                OnLocationLerpFinish?.Invoke();
                isMoving = false;
            }
        }
    }

    public void MovePlayerLerp(Vector2 destination, float speed)
    {
        if ((Vector2)transform.position != destination)
        {
            startPosition = transform.position;
            isMoving = true;
            locationDestination = destination;
            locationLerpSpeed = speed;
        }
    }

    public void StopPlayerMovement()
    {
        isMoving = false;
    }

    public float PlayerLerpProgress()
    {
        if (!isMoving) return 1f;
        
        float totalDistance = Vector2.Distance(startPosition, locationDestination);
        float currentDistance = Vector2.Distance(startPosition, transform.position);
        return totalDistance > 0 ? Mathf.Clamp01(currentDistance / totalDistance) : 1f;
    }
}
