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
    
    void Start()
    {
        SetupCameras();
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
}
