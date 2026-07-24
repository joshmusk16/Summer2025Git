using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.U2D;

public class TimeSpeedUI : SpriteSampling
{

public GameObject emptyObject;
public SpriteAtlas numberAtlas;
private GameObject numberParent;
private Dictionary<string, float> numberWidths = new Dictionary<string, float>();

private GameObject gameSpeedSymbolObject;
private GameObject gameSpeedSymbolParent;
public Sprite gameSpeedSymbolSprite;

private GameObject gameSpeedUIParent;

private List<GameObject> Digits = new();
private List<string> StringDigits = new();

private const float Y_OFFSET = 0.45f;
private const float X_OFFSET = -1.3f;

private const float NUMBER_OFFSET_LENGTH = 3f;
private const float SYMBOL_TO_NUMBER_OFFSET = 6f;
private const float TEXT_SCALE = 0.65f;
private const float SYMBOL_SCALE = 1f;
private const float TEXT_PPU = 16f;
private const int SORTING_ORDER = 100;

public static event Action OnGameSpeedUpdate;

void Awake()
{
    FindNumberWidths();
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.B))
    {
        UpdateGameSpeedUI(UnityEngine.Random.Range(0f, 5f));       
    }
}

public void UpdateGameSpeedUI(float input)
{
    GenerateGameSpeedSymbol();
    GenerateGameSpeedNumber(input);

    if(gameSpeedUIParent == null)
    {
        gameSpeedUIParent = Instantiate(emptyObject, gameObject.transform);
        gameSpeedUIParent.name = "Game Speed Parent";
    }

    numberParent.transform.position = gameSpeedSymbolParent.transform.position 
    + new Vector3(SYMBOL_TO_NUMBER_OFFSET / TEXT_PPU * TEXT_SCALE, 0);

    gameSpeedUIParent.transform.position = gameSpeedSymbolParent.transform.position;

    gameSpeedSymbolParent.transform.SetParent(gameSpeedUIParent.transform);
    numberParent.transform.SetParent(gameSpeedUIParent.transform);

    gameSpeedUIParent.transform.localPosition = Vector3.zero
    + new Vector3(X_OFFSET, Y_OFFSET, 0);

    OnGameSpeedUpdate?.Invoke();
}

private void GenerateGameSpeedNumber(float number)
{
    GetGameSpeedDigits(number);
    DestroyDigits();

    numberParent = Instantiate(emptyObject, gameObject.transform);

    int numberOfDigits = StringDigits.Count;
    Vector2 offset = Vector2.zero;

    for(int i = 0; i < numberOfDigits; i++)
    {
        GameObject newDigit = Instantiate(emptyObject, (Vector2)gameObject.transform.position + offset, 
        Quaternion.identity, gameObject.transform);
        newDigit.transform.localScale = Vector3.one * TEXT_SCALE;
        newDigit.name = StringDigits[i].ToString();
        Digits.Add(newDigit);
        
        SpriteRenderer newDigitSpriteRenderer = newDigit.GetComponent<SpriteRenderer>();
        newDigitSpriteRenderer.sortingOrder = SORTING_ORDER;
        newDigitSpriteRenderer.sprite = numberAtlas.GetSprite(newDigit.name);
        
        float letterWidth = numberWidths[newDigit.name] / 2;
        if(i != numberOfDigits - 1 && numberWidths.ContainsKey(StringDigits[i + 1]))
        {
            letterWidth += numberWidths[StringDigits[i + 1]] / 2f;   
        }

        offset += new Vector2((letterWidth + (NUMBER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0);
    }

    numberParent.transform.position = GetTightBottomLeft(numberAtlas.GetSprite(StringDigits[0]), Digits[0].transform);
    numberParent.name = "Number Parent";

    foreach(GameObject digit in Digits)
    {
        digit.transform.SetParent(numberParent.transform);
    }
}

private void GenerateGameSpeedSymbol()
{
    if(gameSpeedSymbolParent == null)
    {
        gameSpeedSymbolObject = Instantiate(emptyObject, gameObject.transform);
        gameSpeedSymbolParent = Instantiate(emptyObject, gameObject.transform);
        gameSpeedSymbolParent.name = "GameSpeed Symbol Parent";
        gameSpeedSymbolObject.name = "GameSpeed Symbol";
        
        gameSpeedSymbolObject.transform.localScale = Vector3.one * SYMBOL_SCALE;;
        SpriteRenderer gameSpeedSR = gameSpeedSymbolObject.GetComponent<SpriteRenderer>();
        gameSpeedSR.sprite = gameSpeedSymbolSprite;
        gameSpeedSR.sortingOrder = SORTING_ORDER;

        gameSpeedSymbolParent.transform.position = GetTightBottomRight(gameSpeedSymbolSprite, gameSpeedSymbolObject.transform);
        gameSpeedSymbolObject.transform.SetParent(gameSpeedSymbolParent.transform);
    }
}

public void GetGameSpeedDigits(float number)
{
    StringDigits.Clear();

    int totalHundredths = Mathf.RoundToInt(number * 100f);

    int tens = totalHundredths / 1000 % 10;
    int ones = totalHundredths / 100 % 10;

    int tenths = totalHundredths / 10 % 10;
    int hundredths = totalHundredths % 10;

    if (tens != 0)
    {
        StringDigits.Add(tens.ToString());
    }

    StringDigits.Add(ones.ToString());
    StringDigits.Add(".");
    StringDigits.Add(tenths.ToString());
    StringDigits.Add(hundredths.ToString());
    StringDigits.Add("x");
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

private void DestroyDigits()
{
    if(Digits.Count == 0) return;

    foreach(GameObject digit in Digits)
    {
        Destroy(digit);
    }
    Digits.Clear();

    if(numberParent != null) Destroy(numberParent);
}

}
