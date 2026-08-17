using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.U2D;

public class SymbolTextElement : SpriteSampling
{

public string uiElementName;

public GameObject emptyObject;
public SpriteAtlas numberAtlas;
private GameObject textParent;
private Dictionary<string, float> numberWidths = new Dictionary<string, float>();

private GameObject symbolObject;
private GameObject symbolParent;
public Sprite symbolSprite;

private GameObject uiElementParent;

private List<GameObject> TextObjects = new();
private List<string> StringCharacters = new();

public bool isUsingSymbol = true;
public bool isUsingText = true;

private float uiWidth = 0;
private float uiHeight = 0;
private float rightmostX = 0;

[Header("UI Parameters")]
public float Y_OFFSET;
public float X_OFFSET;
public float TEXT_OFFSET_LENGTH;
public float SYMBOL_TO_TEXT_OFFSET;
public float TEXT_SCALE;
public float SYMBOL_SCALE;
public int SORTING_ORDER;

public const float TEXT_PPU = 16f;

public static event Action OnUIUpdate;

public void UpdateUIElement(string input)
{
    if(isUsingSymbol == false && isUsingText == false) return;  

    if(numberWidths.Count == 0) FindNumberWidths();

    if(uiElementParent == null)
    {
        uiElementParent = Instantiate(emptyObject, gameObject.transform);
        uiElementParent.name = uiElementName;
    }

    if(isUsingSymbol) GenerateSymbol();
    if(isUsingText) GenerateText(input);

    if(isUsingSymbol && isUsingText)
    {
        textParent.transform.position = (Vector3)GetTightBottomRight(symbolSprite, symbolObject.transform) 
        + new Vector3(SYMBOL_TO_TEXT_OFFSET / TEXT_PPU * TEXT_SCALE, 0);

        uiElementParent.transform.position = symbolParent.transform.position;

        symbolParent.transform.SetParent(uiElementParent.transform);
        textParent.transform.SetParent(uiElementParent.transform);

        rightmostX = GetTightBottomRight(numberAtlas.GetSprite(StringCharacters[^1]), TextObjects[^1].transform).x;
        uiHeight = Mathf.Max(GetTightHeight(symbolSprite) * SYMBOL_SCALE, GetTightHeight(numberAtlas.GetSprite(StringCharacters[0])) * TEXT_SCALE);

    }
    else if(isUsingText && isUsingSymbol == false)
    {
        uiElementParent.transform.position = textParent.transform.position;
        textParent.transform.SetParent(uiElementParent.transform);
        uiHeight = GetTightHeight(numberAtlas.GetSprite(StringCharacters[0])) * TEXT_SCALE;
    }
    else if(isUsingSymbol && isUsingText == false)
    {
        uiElementParent.transform.position = symbolParent.transform.position;
        symbolParent.transform.SetParent(uiElementParent.transform);
        uiHeight = GetTightHeight(symbolSprite) * SYMBOL_SCALE;
    }

    Debug.Log(rightmostX);

    uiWidth = Mathf.Abs(rightmostX - uiElementParent.transform.position.x);

    uiElementParent.transform.localPosition = Vector3.zero
    + new Vector3(X_OFFSET, Y_OFFSET, 0);

    OnUIUpdate?.Invoke();
}

private void GenerateText(string input)
{
    GetCharactersFromString(input);
    DestroyTextObjects();

    textParent = Instantiate(emptyObject, gameObject.transform);

    int numberOfDigits = StringCharacters.Count;
    Vector2 offset = Vector2.zero;

    for(int i = 0; i < numberOfDigits; i++)
    {
        GameObject newCharacter = Instantiate(emptyObject, (Vector2)gameObject.transform.position + offset, 
        Quaternion.identity, gameObject.transform);
        newCharacter.transform.localScale = Vector3.one * TEXT_SCALE;
        newCharacter.name = StringCharacters[i].ToString();
        TextObjects.Add(newCharacter);
        
        SpriteRenderer newDigitSpriteRenderer = newCharacter.GetComponent<SpriteRenderer>();
        newDigitSpriteRenderer.sortingOrder = SORTING_ORDER;
        newDigitSpriteRenderer.sprite = numberAtlas.GetSprite(newCharacter.name);
        
        float letterWidth = numberWidths[newCharacter.name] / 2;
        if(i != numberOfDigits - 1 && numberWidths.ContainsKey(StringCharacters[i + 1]))
        {
            letterWidth += numberWidths[StringCharacters[i + 1]] / 2f;   
        }

        offset += new Vector2((letterWidth + (TEXT_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0);
    }

    textParent.transform.position = GetTightBottomLeft(numberAtlas.GetSprite(StringCharacters[0]), TextObjects[0].transform);
    rightmostX = GetTightBottomRight(numberAtlas.GetSprite(StringCharacters[^1]), TextObjects[^1].transform).x;
    textParent.name = "Number Parent";

    foreach(GameObject character in TextObjects)
    {
        character.transform.SetParent(textParent.transform);
    }
}

private void GenerateSymbol()
{
    if(symbolParent == null)
    {
        symbolObject = Instantiate(emptyObject, gameObject.transform);
        symbolParent = Instantiate(emptyObject, gameObject.transform);
        symbolParent.name = "Symbol Parent";
        symbolObject.name = "Symbol";
        
        symbolObject.transform.localScale = Vector3.one * SYMBOL_SCALE;;
        SpriteRenderer gameSpeedSR = symbolObject.GetComponent<SpriteRenderer>();
        gameSpeedSR.sprite = symbolSprite;
        gameSpeedSR.sortingOrder = SORTING_ORDER;

        symbolParent.transform.position = GetTightBottomLeft(symbolSprite, symbolObject.transform);
        symbolObject.transform.SetParent(symbolParent.transform);

        if(isUsingText == false) rightmostX = GetTightBottomRight(symbolSprite, symbolObject.transform).x;
    }
}

public void GetCharactersFromString(string input)
{
    StringCharacters.Clear();

    foreach (char c in input)
    {
        StringCharacters.Add(c.ToString());
    }
}

void FindNumberWidths()
{
    numberWidths.Clear();

    Sprite[] sprites = new Sprite[numberAtlas.spriteCount];
    numberAtlas.GetSprites(sprites);

    foreach (Sprite sprite in sprites)
    {
        string name = sprite.name.Replace("(Clone)", "").Trim();
        if (name.Length == 1)
        numberWidths[name] = GetTightWidth(sprite);
    }
}

private void DestroyTextObjects()
{
    if(TextObjects.Count == 0) return;

    foreach(GameObject character in TextObjects)
    {
        Destroy(character);
    }
    TextObjects.Clear();

    if(textParent != null) Destroy(textParent);
}

public float GetWidth()
{
    return uiWidth;
}

public float GetHeight()
{
    return uiHeight;
}

public void MoveUIElement(Vector2 displacement)
{
    if(uiElementParent == null) return;
    uiElementParent.transform.position += (Vector3)displacement; 
}

}
