using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundTransition : MonoBehaviour
{
public DualCameraManager cameraManager;

public Camera uiCamera;

public GameObject topLeftObject;
public GameObject topRightObject;
public GameObject bottomLeftObject;
public GameObject bottomRightObject;

private List<Image> imageComponents = new();

private Vector2 screenCenterPosition; //Destination for all objects translations
private DualCameraManager.ScreenCorners uiScreenCorners;
private Vector2 desiredRectScale;

private const float TRANSITION_ANIMATION_SPEED = 5.0f;

private void FindDependencies()
{
    cameraManager = FindObjectOfType<DualCameraManager>();
}

public void AnimateRoundTransitionIn()
{
    EnableAllImages();

}

public void AnimateRoundTransitionOut()
{
    
}

private void GetAllImageComponents()
{
    if(imageComponents.Count != 0) return;

    imageComponents.Clear();
    imageComponents.Add(topLeftObject.GetComponent<Image>());
    imageComponents.Add(topRightObject.GetComponent<Image>());
    imageComponents.Add(bottomLeftObject.GetComponent<Image>());
    imageComponents.Add(bottomRightObject.GetComponent<Image>());
}

private void EnableAllImages()
{
    if(imageComponents.Count == 0) GetAllImageComponents();

    foreach(Image imageComponent in imageComponents)
    {
        imageComponent.enabled = true;
    }
}

private void DisableAllImages()
{
    if(imageComponents.Count == 0) GetAllImageComponents();

    foreach(Image imageComponent in imageComponents)
    {
        imageComponent.enabled = false;
    }
}

private void GetUIPositions()
{
    if(cameraManager == null) FindDependencies();

    screenCenterPosition = uiCamera.transform.position;
    uiScreenCorners = cameraManager.GetScreenCorners(uiCamera);

    float xValueToScale = Mathf.Abs(screenCenterPosition.x - uiScreenCorners.bottomLeft.x);
    float yValueToScale = Mathf.Abs(screenCenterPosition.y - uiScreenCorners.bottomLeft.y);

    desiredRectScale = new Vector2(xValueToScale, yValueToScale);
}

}
