using System;
using UnityEngine;

public class DualCameraManager : MonoBehaviour
{
    [Header("Camera References")]
    public Camera mainCamera;      // Your game camera that moves/zooms
    public Camera uiCamera;        // Static UI camera
    
    [Header("Layer Settings")]
    public LayerMask gameplayLayers = -1;  // What the main camera sees
    public LayerMask uiLayers = 0;         // What the UI camera sees (set in inspector)
    
    [Header("UI Camera Settings")]
    public float uiCameraSize = 5f;        // Fixed orthographic size for UI camera
    public float uiCameraDistance = 10f;   // Distance from UI elements
    
    private bool isZooming = false;
    private float zoomDestination;
    private float zoomLerpSpeed;
    public event Action OnZoomLerpFinish;

    public struct ScreenCorners
    {
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
        public Vector3 topLeft;
        public Vector3 topRight;
    }

    void Start()
    {
        SetupCameras();
    }
    
    void Update()
    {
        if (isZooming)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomDestination, Time.deltaTime * zoomLerpSpeed);

            if (Mathf.Abs(mainCamera.fieldOfView - zoomDestination) < 0.01f)
            {
                mainCamera.fieldOfView = zoomDestination;
                OnZoomLerpFinish?.Invoke();
                isZooming = false;
            }
        }
    }

    void SetupCameras()
    {
        // Configure Main Camera (Game Camera)
        if (mainCamera != null)
        {
            // Main camera renders everything EXCEPT UI layers
            mainCamera.cullingMask = gameplayLayers & ~uiLayers;
            mainCamera.depth = 0; // Renders first
        }
        
        // Configure UI Camera
        if (uiCamera != null)
        {
            // UI camera ONLY renders UI layers
            uiCamera.cullingMask = uiLayers;
            uiCamera.depth = 1; // Renders on top of main camera
            
            // Keep UI camera orthographic and static
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = uiCameraSize;
            
            // Position UI camera to look at UI elements
            uiCamera.transform.position = new Vector3(0, 0, -uiCameraDistance);
            uiCamera.transform.rotation = Quaternion.identity;
            
            // Clear flags - only clear depth so UI renders over game
            uiCamera.clearFlags = CameraClearFlags.Depth;
        }
    }
    
    // Call this if you need to adjust UI camera settings at runtime
    public void UpdateUICameraSize(float newSize)
    {
        if (uiCamera != null)
        {
            uiCamera.orthographicSize = newSize;
        }
    }


    public void ZoomMainCameraLerp(float destination, float speed)
    {
        if (mainCamera.fieldOfView != destination)
        {
            isZooming = true;
            zoomDestination = destination;
            zoomLerpSpeed = speed;
        }
    }

    public ScreenCorners GetScreenCorners(Camera cam)
    {
        float depth = -cam.transform.position.z; // distance to gameplay plane (z = 0)
        
        ScreenCorners corners = new ScreenCorners
        {
            bottomLeft  = cam.ScreenToWorldPoint(new Vector3(0f, 0f, depth)),
            bottomRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, depth)),
            topLeft     = cam.ScreenToWorldPoint(new Vector3(0f, Screen.height, depth)),
            topRight    = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth))
        };

        return corners;
    }

}
