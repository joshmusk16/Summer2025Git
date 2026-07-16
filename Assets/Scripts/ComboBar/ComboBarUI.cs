using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.U2D;

public class ComboBarUI : SpriteSampling
{

public GameObject emptyObject;
public SpriteAtlas numberAtlas;
private GameObject numberParent;
private Dictionary<string, float> numberWidths = new Dictionary<string, float>();

private GameObject damageSymbolObject;
private GameObject damageSymbolParent;
public Sprite damageSymbolSprite;

private List<GameObject> Digits = new();
private List<int> IntegerDigits = new();
private const float Y_OFFSET = 1.3f;

private const float NUMBER_OFFSET_LENGTH = 2f;
private const float SYMBOL_TO_NUMBER_OFFSET = 4f;
private const float TEXT_SCALE = 1f;
private const float TEXT_PPU = 16f;
private const int SORTING_ORDER = 100;

public static event Action OnComboUpdate;

void Awake()
{
    FindNumberWidths();
}

//Update for debugging
void Update()
{
    if (Input.GetKeyDown(KeyCode.N))
    {
        GenerateComboNumber(UnityEngine.Random.Range(1, 200));        
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

public void GenerateComboNumber(int number)
{
    DestroyDigits();
    AddDigitsToList(number);
    numberParent = Instantiate(emptyObject, gameObject.transform);

    int numberOfDigits = CountDigits(number);
    Vector2 offset = new(0f, -Y_OFFSET);

    for(int i = 0; i < numberOfDigits; i++)
    {
        GameObject newDigit = Instantiate(emptyObject, (Vector2)gameObject.transform.position + offset, 
        Quaternion.identity, gameObject.transform);
        newDigit.transform.localScale = Vector3.one * TEXT_SCALE;
        newDigit.name = IntegerDigits[i].ToString();
        Digits.Add(newDigit);
        
        SpriteRenderer newDigitSpriteRenderer = newDigit.GetComponent<SpriteRenderer>();
        newDigitSpriteRenderer.sortingOrder = SORTING_ORDER;
        newDigitSpriteRenderer.sprite = numberAtlas.GetSprite(newDigit.name);
        
        float letterWidth = numberWidths[newDigit.name] / 2;
        if(i != numberOfDigits - 1 && numberWidths.ContainsKey(IntegerDigits[i + 1].ToString()))
        {
            letterWidth += numberWidths[IntegerDigits[i + 1].ToString()] / 2f;   
        }

        offset += new Vector2((letterWidth + (NUMBER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0);
    }

    numberParent.transform.position = GetTightBottomLeft(numberAtlas.GetSprite(IntegerDigits[0].ToString()), Digits[0].transform);
    numberParent.name = "Number Parent";

    foreach(GameObject digit in Digits)
    {
        digit.transform.SetParent(numberParent.transform);
    }

    numberParent.transform.position -= (Vector3)offset / 2f;

    OnComboUpdate?.Invoke();
}

private void GenerateDamageSymbol()
{
    if(damageSymbolParent == null)
    {
        damageSymbolObject = Instantiate(emptyObject, gameObject.transform);
        damageSymbolParent = Instantiate(emptyObject, gameObject.transform);
        damageSymbolParent.name = "Damage Symbol Parent";
        damageSymbolObject.name = "Damage Symbol";
        
        damageSymbolObject.transform.localScale = Vector3.one * TEXT_SCALE;
        SpriteRenderer dmgSR = damageSymbolObject.GetComponent<SpriteRenderer>();
        dmgSR.sprite = damageSymbolSprite;
        dmgSR.sortingOrder = SORTING_ORDER;

        damageSymbolParent.transform.position = GetTightBottomLeft(damageSymbolSprite, damageSymbolObject.transform);
        damageSymbolObject.transform.SetParent(damageSymbolParent.transform);
    }
}

public void UpdateComboNumberUI()
{
    //GenerateDamageSymbol and GenerateComboNumber, then reposition to center on the combar bar...
}


private int CountDigits(int number)
{
    if (number == 0) return 1;
    return (int)Mathf.Floor(Mathf.Log10(Mathf.Abs(number))) + 1; 
}

private void AddDigitsToList(int number)
{
    IntegerDigits.Clear();
    string numStr = Mathf.Abs(number).ToString();
    
    foreach (char c in numStr)
        IntegerDigits.Add(c - '0');
}

private void DestroyDigits()
{
    foreach(GameObject digit in Digits)
    {
        Destroy(digit);
    }
    Digits.Clear();
    IntegerDigits.Clear();

    if(numberParent != null) Destroy(numberParent);
}

}
