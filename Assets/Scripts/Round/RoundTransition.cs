using System.Collections.Generic;
using UnityEngine;

public class RoundTransition : MonoBehaviour
{
public DualCameraManager cameraManager;

public Camera uiCamera;

public GameObject topLeftObject;
public GameObject topRightObject;
public GameObject bottomLeftObject;
public GameObject bottomRightObject;

private List<SpriteRenderer> spriteRenderers = new();

private Vector2 screenCenterPosition; //Destination for all objects translations
private DualCameraManager.ScreenCorners uiScreenCorners;

private const float TRANSITION_ANIMATION_SPEED = 5.0f;

private void FindDependencies()
{
    cameraManager = FindObjectOfType<DualCameraManager>();
}

public void AnimateRoundTransitionIn()
{
    
}

public void AnimateRoundTransitionOut()
{
    
}

private void GetAllSpriteRenderers()
{
    if(spriteRenderers.Count != 0) return;

    spriteRenderers.Clear();
    spriteRenderers.Add(topLeftObject.GetComponent<SpriteRenderer>());
    spriteRenderers.Add(topRightObject.GetComponent<SpriteRenderer>());
    spriteRenderers.Add(bottomLeftObject.GetComponent<SpriteRenderer>());
    spriteRenderers.Add(bottomRightObject.GetComponent<SpriteRenderer>());
}

private void EnableAllSpriteRenderers()
{
    if(spriteRenderers.Count == 0) GetAllSpriteRenderers();

    foreach(SpriteRenderer renderer in spriteRenderers)
    {
        renderer.enabled = true;
    }
}

private void DisableAllSpriteRenderers()
{
    if(spriteRenderers.Count == 0) GetAllSpriteRenderers();

    foreach(SpriteRenderer renderer in spriteRenderers)
    {
        renderer.enabled = false;
    }
}

private void GetUIPositions()
{
    if(cameraManager == null) FindDependencies();

    screenCenterPosition = uiCamera.transform.position;
    uiScreenCorners = cameraManager.GetScreenCorners(uiCamera);
}

}
