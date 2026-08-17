using UnityEngine;

public class ButtonUIElement : MonoBehaviour
{

public string textInput;
public SymbolTextElement symbolTextElement;
public float buttonOffsetBuffer = 0;

private MouseTracker mouseTracker;
private RectTransform rect;
private float buttonWidth = 0;
private float buttonHeight = 0;

void Awake()
{
    GenerateButton();
}

private void FindDependencies()
{
    if(mouseTracker != null && rect != null) return;
    mouseTracker = FindObjectOfType<MouseTracker>();
    rect = gameObject.GetComponent<RectTransform>();
}

private void GenerateButton()
{
    FindDependencies();

    symbolTextElement.UpdateUIElement(textInput);

    buttonWidth = symbolTextElement.GetWidth();
    buttonHeight = symbolTextElement.GetHeight();

    symbolTextElement.MoveUIElement(-new Vector2(buttonWidth / 2f, buttonHeight / 2f));

    rect.sizeDelta = new Vector2(buttonWidth + (buttonOffsetBuffer * 2), buttonHeight + (buttonOffsetBuffer * 2));
}


}
