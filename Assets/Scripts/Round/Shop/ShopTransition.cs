using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopTransition : MonoBehaviour
{

public Camera uiCamera;
private DualCameraManager cameraManager;
private DualCameraManager.ScreenCorners uiScreenCorners; 

public GameObject leftObjectPrefab;
public GameObject rightObjectPrefab;
private GameObject leftObject;
private GameObject rightObject;

private RectTransform rightRect;
private RectTransform leftRect;
private LerpUIHandler leftLerpHandler;
private LerpUIHandler rightLerpHandler;
private Image leftImageComponent;
private Image rightImageComponent;

private const float Y_UI_SCALE = 10f;
private const float TRANSITION_IN_ANIMATION_SPEED = 10.0f;
private const float TRANSITION_OUT_ANIMATION_SPEED = 5.0f;
private Vector2 transitionOutScale = new(1, Y_UI_SCALE);

private const float Y_UI_OFFSET = 0f;
private Vector2 leftStartPos;
private Vector2 rightStartPos;
private Vector2 screenCenterPosition;
private Vector2 desiredRectScale;

public event Action OnAnimationInFinish;
public event Action OnAnimationOutFinish;

//update for debugging
private void Update()
{
    if (Input.GetKeyDown(KeyCode.B))
    {
        AnimateRoundTransitionIn();
    }   

    if (Input.GetKeyDown(KeyCode.N))
    {
        AnimateRoundTransitionOut(); 
    }   
}
//update for debugging

private void FindDependencies()
{
    if(cameraManager == null) cameraManager = FindObjectOfType<DualCameraManager>();
}

private void GetUIPositions()
{
    FindDependencies();

    screenCenterPosition = uiCamera.transform.position;
    uiScreenCorners = cameraManager.GetScreenCorners(uiCamera);

    leftStartPos = uiScreenCorners.middleLeft + new Vector3(0, Y_UI_OFFSET);
    rightStartPos = uiScreenCorners.middleRight + new Vector3(0, Y_UI_OFFSET);

    float xDistance = Mathf.Abs(screenCenterPosition.x - leftStartPos.x);
    desiredRectScale = new Vector2(xDistance, Y_UI_SCALE); 
}

public void AnimateRoundTransitionIn()
{
    EnableAllImages();

    if(leftObject == null || rightObject == null) InitializeTransitionObjects();

    rightLerpHandler.RectTransformScaleLerp(desiredRectScale, TRANSITION_IN_ANIMATION_SPEED);
    leftLerpHandler.RectTransformScaleLerp(desiredRectScale, TRANSITION_IN_ANIMATION_SPEED);
    
    leftLerpHandler.OnRectTransformScaleFinish += AnimationInFinished;
}

public void AnimateRoundTransitionOut()
{
    rightLerpHandler.RectTransformScaleLerp(transitionOutScale, TRANSITION_OUT_ANIMATION_SPEED);
    leftLerpHandler.RectTransformScaleLerp(transitionOutScale, TRANSITION_OUT_ANIMATION_SPEED);

    leftLerpHandler.OnRectTransformScaleFinish += AnimationOutFinished;
}

private void InitializeTransitionObjects()
{
    DestroyTransitionObjects();
    GetUIPositions();

    rightObject = Instantiate(rightObjectPrefab, rightStartPos, Quaternion.identity, gameObject.transform);
    leftObject = Instantiate(leftObjectPrefab, leftStartPos, Quaternion.identity, gameObject.transform);

    rightRect = rightObject.GetComponent<RectTransform>();
    leftRect = leftObject.GetComponent<RectTransform>();
    leftLerpHandler = leftObject.GetComponent<LerpUIHandler>();
    rightLerpHandler = rightObject.GetComponent<LerpUIHandler>();
    leftImageComponent = leftObject.GetComponent<Image>();
    rightImageComponent = rightObject.GetComponent<Image>();

    rightRect.sizeDelta = transitionOutScale;
    leftRect.sizeDelta = transitionOutScale;
}

private void EnableAllImages()
{
    if(leftImageComponent == null || rightImageComponent == null) return;

    leftImageComponent.enabled = true;
    rightImageComponent.enabled = true;
}

private void DisableAllImages()
{
    if(leftImageComponent == null || rightImageComponent == null) return;   
    
    leftImageComponent.enabled = false;
    rightImageComponent.enabled = false;
}

public void AnimationInFinished()
{
    OnAnimationInFinish?.Invoke();
    leftLerpHandler.OnRectTransformScaleFinish -= AnimationInFinished;
}

private void AnimationOutFinished()
{
    OnAnimationOutFinish?.Invoke();
    leftLerpHandler.OnRectTransformScaleFinish -= AnimationOutFinished;
}

private void DestroyTransitionObjects()
{
    if(rightObject == null) return;
    if(leftObject == null) return;

    leftLerpHandler = null;
    rightLerpHandler = null;
    leftImageComponent = null;
    rightImageComponent = null;
    Destroy(rightObject);
    Destroy(leftObject);     
}

private void OnDestroy()
{
    if(leftLerpHandler == null) return;

    leftLerpHandler.OnRectTransformScaleFinish -= AnimationInFinished;
    leftLerpHandler.OnRectTransformScaleFinish -= AnimationOutFinished;   
}


}
