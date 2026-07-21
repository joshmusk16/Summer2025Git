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
private List<int> IntegerDigits = new();

private const float Y_OFFSET = 1.15f;
private const float NUMBER_OFFSET_LENGTH = 2f;
private const float SYMBOL_TO_NUMBER_OFFSET = 3f;
private const float TEXT_SCALE = 1f;
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
        //UpdateGameSpeedUI(UnityEngine.Random.Range(1, 200));       
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

private void GenerateGameSpeedNumber(float number)
{
    GetGameSpeedDigits(number);

    
}

private void GenerateGameSpeedSymbol()
{
    if(gameSpeedSymbolParent == null)
    {
        gameSpeedSymbolObject = Instantiate(emptyObject, gameObject.transform);
        gameSpeedSymbolParent = Instantiate(emptyObject, gameObject.transform);
        gameSpeedSymbolParent.name = "GameSpeed Symbol Parent";
        gameSpeedSymbolObject.name = "GameSpeed Symbol";
        
        gameSpeedSymbolObject.transform.localScale = Vector3.one * TEXT_SCALE;
        SpriteRenderer gameSpeedSR = gameSpeedSymbolObject.GetComponent<SpriteRenderer>();
        gameSpeedSR.sprite = gameSpeedSymbolSprite;
        gameSpeedSR.sortingOrder = SORTING_ORDER;

        gameSpeedSymbolParent.transform.position = GetTightBottomLeft(gameSpeedSymbolSprite, gameSpeedSymbolObject.transform);
        gameSpeedSymbolObject.transform.SetParent(gameSpeedSymbolParent.transform);
    }
}

public void GetGameSpeedDigits(float number)
{
    IntegerDigits.Clear();

    int totalHundredths = Mathf.RoundToInt(number * 100f);

    int tens = totalHundredths / 1000 % 10;
    int ones = totalHundredths / 100 % 10;
    int tenths = totalHundredths / 10 % 10;
    int hundredths = totalHundredths % 10;

    if (tens != 0)
    {
        IntegerDigits.Add(tens);
    }

    IntegerDigits.Add(ones);
    IntegerDigits.Add(tenths);
    IntegerDigits.Add(hundredths);
}



}
