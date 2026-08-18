using UnityEngine;
using System;

public class ButtonUIElement : MonoBehaviour
{

public string textInput;
public SymbolTextElement symbolTextElement;
public float buttonOffsetBuffer = 0;
public bool buttonIsActive = true;

private MouseTracker mouseTracker;
private RectTransform rect;
private LerpUIHandler lerpUIHandler;
private float buttonWidth = 0;
private float buttonHeight = 0;

public event Action OnButtonPressed;

void Awake()
{
    GenerateButton();
}

void Update() 
{
    if (buttonIsActive)
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Vector2 mousePos = mouseTracker.GetUIMousePosition();
            Vector2 buttonPos = gameObject.transform.position;
            float halfWidth = buttonWidth / 2f;
            float halfHeight = buttonHeight / 2f;

            if(mousePos.x < buttonPos.x + halfWidth && mousePos.x > buttonPos.x - halfWidth &&
            mousePos.y < buttonPos.y + halfHeight && mousePos.y > buttonPos.y - halfHeight)
            {
                OnButtonPressed?.Invoke();
                lerpUIHandler.ParabolicScaleLerp(new Vector2(1.1f, 1.1f), 0.15f, 1.3f);
            }
        }
    }
}

private void FindDependencies()
{
    if(mouseTracker != null && rect != null
    && lerpUIHandler != null) return;

    mouseTracker = FindObjectOfType<MouseTracker>();
    rect = gameObject.GetComponent<RectTransform>();
    lerpUIHandler = gameObject.GetComponent<LerpUIHandler>();
}

private void GenerateButton()
{
    FindDependencies();

    buttonWidth = 0;
    buttonHeight = 0;

    symbolTextElement.UpdateUIElement(textInput);

    buttonWidth = symbolTextElement.GetWidth();
    buttonHeight = symbolTextElement.GetHeight();

    symbolTextElement.MoveUIElement(-new Vector2(buttonWidth / 2f, buttonHeight / 2f));

    rect.sizeDelta = new Vector2(buttonWidth + (buttonOffsetBuffer * 2), buttonHeight + (buttonOffsetBuffer * 2));

    buttonWidth += buttonOffsetBuffer * 2;
    buttonHeight += buttonOffsetBuffer * 2;
}


}
