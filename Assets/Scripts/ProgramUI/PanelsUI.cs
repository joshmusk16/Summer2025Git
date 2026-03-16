using UnityEngine;

public class PanelsUI : MonoBehaviour
{
public ProgramInputManager programInputManager;    
public MouseTracker mouse;
public DualCameraManager cameraManager;

public SpriteRenderer leftUIPanel;
public SpriteRenderer rightUIPanel;

public LerpUIHandler leftPanelLerp;
public LerpUIHandler rightPanelLerp;

private const float LEFT_PANEL_X = -21f;
private const float RIGHT_PANEL_X = 21f;

// public Vector2 leftPanelStartPos;
// public Vector2 rightPanelStartPos;
// private Vector2 newLeftPanelLerpPos;
// private Vector2 newRightPanelLerpPos;

// private const float PANEL_LERP_OFFSET = 0.5f;
// private const float PANEL_LERP_SPEED = 10f;

private const float ZOOM_LERP_START = 118f;
private const float ZOOM_LERP_DESTINATION = 122f;
private const float ZOOM_LERP_SPEED = 8f;

public Sprite rightPanelActiveSprite;
public Sprite rightPanelInactiveSprite;
public Sprite leftPanelActiveSprite;
public Sprite leftPanelInactiveSprite;

public bool isInLeftOrRightZone = false;

void Start()
{
    // leftPanelStartPos = leftUIPanel.gameObject.transform.position;
    // rightPanelStartPos = rightUIPanel.gameObject.transform.position;

    // newLeftPanelLerpPos = leftPanelStartPos + new Vector2(PANEL_LERP_OFFSET, 0);
    // newRightPanelLerpPos = rightPanelStartPos - new Vector2(PANEL_LERP_OFFSET, 0);

    if(programInputManager != null)
    {
        programInputManager.OnSlowModeEnter += LerpPanelsIn;
        programInputManager.OnSlowModeExit += LerpPanelsOut;
    }
}

void Update()
{
    bool currentlyInZone = mouse.uiPosition.x < LEFT_PANEL_X || mouse.uiPosition.x > RIGHT_PANEL_X;

    if (currentlyInZone != isInLeftOrRightZone)
    {
        isInLeftOrRightZone = currentlyInZone;
        UpdatePanelStates();
    }
}

void UpdatePanelStates()
{
    if(mouse.uiPosition.x < LEFT_PANEL_X)
    {
        leftUIPanel.sprite = leftPanelActiveSprite;
        // leftPanelLerp.LocationLerp(newLeftPanelLerpPos + new Vector2(PANEL_LERP_OFFSET, 0), PANEL_LERP_SPEED);
    }
    else
    {
        leftUIPanel.sprite = leftPanelInactiveSprite;
        // leftPanelLerp.LocationLerp(newLeftPanelLerpPos, PANEL_LERP_SPEED);
    }

    if(mouse.uiPosition.x > RIGHT_PANEL_X)
    {
        rightUIPanel.sprite = rightPanelActiveSprite;
        // rightPanelLerp.LocationLerp(newRightPanelLerpPos - new Vector2(PANEL_LERP_OFFSET, 0), PANEL_LERP_SPEED);
    }
    else
    {
        rightUIPanel.sprite = rightPanelInactiveSprite;
        // rightPanelLerp.LocationLerp(newRightPanelLerpPos, PANEL_LERP_SPEED);
    }
}

void LerpPanelsIn()
{
    cameraManager.ZoomMainCameraLerp(ZOOM_LERP_DESTINATION, ZOOM_LERP_SPEED);
    // leftPanelLerp.LocationLerp(newLeftPanelLerpPos, PANEL_LERP_SPEED);
    // rightPanelLerp.LocationLerp(newRightPanelLerpPos, PANEL_LERP_SPEED);
}

void LerpPanelsOut()
{
    cameraManager.ZoomMainCameraLerp(ZOOM_LERP_START, ZOOM_LERP_SPEED);
    // leftPanelLerp.LocationLerp(leftPanelStartPos, PANEL_LERP_SPEED);  
    // rightPanelLerp.LocationLerp(rightPanelStartPos, PANEL_LERP_SPEED);
}

void OnDestroy()
{
    programInputManager.OnSlowModeEnter -= LerpPanelsIn;
    programInputManager.OnSlowModeExit -= LerpPanelsOut;
}

}
