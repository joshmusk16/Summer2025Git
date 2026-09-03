using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoundTransition : MonoBehaviour
{
public DualCameraManager cameraManager;

public Camera uiCamera;

public GameObject topLeftObject;
public GameObject topRightObject;
public GameObject bottomLeftObject;
public GameObject bottomRightObject;

private List<GameObject> transitionGameObjects = new();
private List<Image> imageComponents = new();
private List<LerpUIHandler> lerpUIHandlers = new();
private LerpUIHandler animationTrackingReference;

private Vector2 screenCenterPosition; //Destination for all objects translations
private DualCameraManager.ScreenCorners uiScreenCorners;
private Vector2 desiredRectScale;

private const float TRANSITION_IN_ANIMATION_SPEED = 10.0f;
private const float TRANSITION_OUT_ANIMATION_SPEED = 5.0f;
private Vector2 transitionOutScale = new(1,1);

public event Action OnAnimationInFinish;
public event Action OnAnimationOutFinish;

//update for debugging
private void Update()
{
    if (Input.GetKeyDown(KeyCode.O))
    {
        AnimateRoundTransitionIn();
    }   

    if (Input.GetKeyDown(KeyCode.I))
    {
        AnimateRoundTransitionOut(); 
    }   
}
//update for debugging

private void FindDependencies()
{
    if(cameraManager == null) cameraManager = FindObjectOfType<DualCameraManager>();
}

public void AnimateRoundTransitionIn()
{
    EnableAllImages();

    if(transitionGameObjects.Count == 0) InitializeTransitionObjects();
    
    foreach(LerpUIHandler lerpHandler in lerpUIHandlers)
    {
        lerpHandler.RectTransformScaleLerp(desiredRectScale, TRANSITION_IN_ANIMATION_SPEED);
    }

    animationTrackingReference.OnRectTransformScaleFinish += AnimationInFinished;
}

public void AnimateRoundTransitionOut()
{
    foreach(LerpUIHandler lerpHandler in lerpUIHandlers)
    {
        lerpHandler.RectTransformScaleLerp(transitionOutScale, TRANSITION_OUT_ANIMATION_SPEED);
    }

    animationTrackingReference.OnRectTransformScaleFinish += AnimationOutFinished;
}

private void InitializeTransitionObjects()
{
    DestroyTransitionObjects();
    GetUIPositions();

    GameObject topLeft = Instantiate(topLeftObject, uiScreenCorners.topLeft, Quaternion.identity, gameObject.transform);
    GameObject topRight = Instantiate(topRightObject, uiScreenCorners.topRight, Quaternion.identity, gameObject.transform);  
    GameObject bottomLeft = Instantiate(bottomLeftObject, uiScreenCorners.bottomLeft, Quaternion.identity, gameObject.transform);  
    GameObject bottomRight = Instantiate(bottomRightObject, uiScreenCorners.bottomRight, Quaternion.identity, gameObject.transform);
    
    transitionGameObjects.Add(topLeft);
    transitionGameObjects.Add(topRight);  
    transitionGameObjects.Add(bottomLeft);  
    transitionGameObjects.Add(bottomRight);  

    GetAllImageComponents();
    GetAllLerpHandlers();
}

private void GetAllImageComponents()
{
    if(imageComponents.Count != 0 || 
    transitionGameObjects.Count == 0) return;

    imageComponents.Clear();
    foreach(GameObject obj in transitionGameObjects)
    {
        imageComponents.Add(obj.GetComponent<Image>());    
    }
}

private void GetAllLerpHandlers()
{
    if(lerpUIHandlers.Count != 0 || 
    transitionGameObjects.Count == 0) return;

    lerpUIHandlers.Clear();
    foreach(GameObject obj in transitionGameObjects)
    {
        lerpUIHandlers.Add(obj.GetComponent<LerpUIHandler>());    
    }

    animationTrackingReference = lerpUIHandlers[0];
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

private void DestroyTransitionObjects()
{
    if(transitionGameObjects.Count == 0) return;

    foreach(GameObject obj in transitionGameObjects)
    {
        Destroy(obj);
    }

    transitionGameObjects.Clear();
}

public void AnimationInFinished()
{
    OnAnimationInFinish?.Invoke();
    animationTrackingReference.OnRectTransformScaleFinish -= AnimationInFinished;
}

private void AnimationOutFinished()
{
    OnAnimationOutFinish?.Invoke();
    animationTrackingReference.OnRectTransformScaleFinish -= AnimationOutFinished;
}

private void OnDestroy()
{
    if(animationTrackingReference == null) return;

    animationTrackingReference.OnRectTransformScaleFinish -= AnimationInFinished;
    animationTrackingReference.OnRectTransformScaleFinish -= AnimationOutFinished;   
}

}
