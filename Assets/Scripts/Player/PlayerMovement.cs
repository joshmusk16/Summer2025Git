using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool isMoving = false;
    private Vector2 locationDestination;
    private Vector2 startPosition;
    private float moveElapsedTime;
    private float moveSpeed;
    private float totalDistance;
    public event Action OnLocationLerpFinish;

    void Update()
    {
        if (isMoving)
        {
            moveElapsedTime += Time.deltaTime * moveSpeed;
            float t = Mathf.Clamp01(moveElapsedTime / totalDistance);
            
            // EaseInQuadratic formula: t^2
            float easedT = t * t;
            
            gameObject.transform.position = Vector2.Lerp(startPosition, locationDestination, easedT);

            if (t >= 1f)
            {
                transform.position = locationDestination;
                OnLocationLerpFinish?.Invoke();
                isMoving = false;
            }
        }
    }

    public void MovePlayerLerp(Vector2 destination, float speed = 1f)
    {
        if ((Vector2)transform.position != destination)
        {
            startPosition = transform.position;
            isMoving = true;
            locationDestination = destination;
            moveSpeed = speed;
            totalDistance = Vector2.Distance(startPosition, destination);
            moveElapsedTime = 0f;
        }
    }

    public void StopPlayerMovement()
    {
        isMoving = false;
    }

    public float PlayerLerpProgress()
    {
        if (!isMoving) return 1f;
        
        float t = Mathf.Clamp01(moveElapsedTime / totalDistance);
        return t * t; // Returns eased progress
    }
}