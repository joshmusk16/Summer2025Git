using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TextElement : SpriteSampling
{

[Header("Text Attributes")]
private GameObject textParent;
public string textInput;
public SpriteAtlas fontAtlas;
public GameObject prefabCharacter;

private List<GameObject> letters = new List<GameObject>();
private const float LETTER_OFFSET_LENGTH = 2f;
private const float SPACE_LENGTH = 1.25f;
private const int SORTING_ORDER = 100;
private const float TEXT_SCALE = 0.5f;
private const float NEW_LINE_OFFSET = 4f;
private const float TEXT_PPU = 16f;

[Header("Header Attributes")]
private GameObject headerParent;
private float headerWidth;
private float headerHeight;
private List<GameObject> headerLetters = new List<GameObject>();
public bool isUsingHeader;
public string headerInput;
private const float HEADER_SCALE = 0.7f;
private const float HEADER_LETTER_OFFSET_LENGTH = 2f;
private const float HEADER_NEW_LINE_OFFSET = 8f;

[Header("Background Attributes")]
public GameObject backgroundPrefab;
private GameObject textBackground;
public bool isUsingBackground = true;
public const float BACKGROUND_BUFFER = 1.25f;
public float maxTextHeight;
public float maxTextWidth;

private Dictionary<string, float> fontWidths = new Dictionary<string, float>();
private float lineHeight = 0;

void Awake()
{
    FindFontWidths();
    FindLineHeight();
}

void FindFontWidths()
{
    fontWidths.Clear();

    Sprite[] sprites = new Sprite[fontAtlas.spriteCount];
    fontAtlas.GetSprites(sprites);

    foreach (Sprite sprite in sprites)
    {
        string name = sprite.name.Replace("(Clone)", "").Trim();
        if (name.Length == 1)
        fontWidths[name] = GetTightWidth(sprite);
    }
}

void FindLineHeight()
{
    lineHeight = 0;
    Sprite[] sprites = new Sprite[fontAtlas.spriteCount];
    fontAtlas.GetSprites(sprites);

    foreach (Sprite sprite in sprites)
    {
        float temp = GetTightHeight(sprite);
        if (lineHeight < temp)
        {
            lineHeight = temp;
        }
    }
}

public void GenerateTextElement()
{
    GenerateText(textInput);

    if (isUsingHeader)
    {
        GenerateHeader(headerInput);
        headerParent.transform.position = gameObject.transform.position;
        maxTextHeight += headerHeight;
        
        if(maxTextWidth < headerWidth)
        {
            maxTextWidth = headerWidth;
        }
    }

    textParent.transform.position = gameObject.transform.position + new Vector3(0, -headerHeight);
    GenerateTextBackground(maxTextWidth, maxTextHeight);
}

public void DestroyTextElement()
{
    if(textParent != null)
    {
        maxTextHeight = 0;
        maxTextWidth = 0; 
        letters.Clear();
        DestroyImmediate(textParent);      
    }

    if (isUsingHeader && headerParent != null)
    {
        headerHeight = 0;
        headerWidth = 0;
        headerLetters.Clear();
        DestroyImmediate(headerParent);       
    }

    if (isUsingBackground)
    {
        DestroyImmediate(textBackground);   
    }
}

public void GenerateHeader(string headerInput)
{
    headerLetters.Clear();
    DestroyImmediate(headerParent);

    Vector3 offset = Vector2.zero;
    headerParent = Instantiate(prefabCharacter, gameObject.transform);
    headerParent.name = "Header";

    GameObject letter = null;
    Sprite sprite = null;

    for (int i = 0; i < headerInput.Length; i++)
        {
            if (headerInput[i] == ' ')
            {
                offset += new Vector3(SPACE_LENGTH * HEADER_SCALE, 0, 0);
                continue;
            }

        string spriteName = char.ToUpper(headerInput[i]).ToString();
        sprite = fontAtlas.GetSprite(spriteName);
        
        letter = Instantiate(prefabCharacter, offset, Quaternion.identity, gameObject.transform);
        letter.name = spriteName;
        letter.transform.localScale = Vector3.one * HEADER_SCALE;
        letter.GetComponent<SpriteRenderer>().sprite = sprite;
        letter.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;
        headerLetters.Add(letter);

        float letterWidth = fontWidths[spriteName] / 2;
        if(i != headerInput.Length - 1 && fontWidths.ContainsKey(char.ToUpper(headerInput[i + 1]).ToString()))
        {
            letterWidth += fontWidths[char.ToUpper(headerInput[i + 1]).ToString()] / 2f;   
        }

        offset += new Vector3((letterWidth + (HEADER_LETTER_OFFSET_LENGTH / TEXT_PPU)) * HEADER_SCALE, 0, 0);
        }

    headerParent.transform.position = GetTightTopLeft(fontAtlas.GetSprite(headerLetters[0].name), headerLetters[0].transform);
    foreach(GameObject character in headerLetters)
    {
        character.transform.SetParent(headerParent.transform);
    }

    headerWidth = Mathf.Abs(headerParent.transform.position.x - GetTightBottomRight(sprite, letter.transform).x);
    headerHeight = (lineHeight + (HEADER_NEW_LINE_OFFSET / TEXT_PPU)) * HEADER_SCALE;
}


public void GenerateText(string input)
{
    letters.Clear();
    DestroyImmediate(textParent);

    Vector3 offset = Vector3.zero;
    maxTextWidth = 0;
    maxTextHeight = 0;

    int numberOfNewLines = 0;

    float xLocation = 0;

    bool lastLetterWasNewLine = false;
    bool lastLetterWasSpace = false;

    textParent = Instantiate(prefabCharacter, gameObject.transform);
    textParent.name = "Text";

    GameObject letter = null;
    Sprite sprite = null;

    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == ' ')
        {
            offset += new Vector3(SPACE_LENGTH * TEXT_SCALE, 0, 0);
            lastLetterWasSpace = true;
            continue;
        }
        else if(input[i] == '/' && i != input.Length - 1)
        {
            if(input[i + 1] == 'N')
            {
                i++;
                numberOfNewLines++;

                if(maxTextWidth < offset.x && letter != null && sprite != null)
                {
                    maxTextWidth = Mathf.Abs(xLocation - GetTightBottomRight(sprite, letter.transform).x);      
                }

                offset = new Vector3(xLocation, (-(lineHeight * numberOfNewLines + NEW_LINE_OFFSET / TEXT_PPU * numberOfNewLines)) * TEXT_SCALE);                
                lastLetterWasNewLine = true;
                continue;
            }
        }

        string spriteName = char.ToUpper(input[i]).ToString();
        sprite = fontAtlas.GetSprite(spriteName);

        letter = Instantiate(prefabCharacter, offset, Quaternion.identity, gameObject.transform);
        letter.name = spriteName;
        letter.transform.localScale = Vector3.one * TEXT_SCALE;
        letter.GetComponent<SpriteRenderer>().sprite = sprite;
        letter.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;
        letters.Add(letter);

        if(i == 0)
        {
            xLocation = GetTightBottomLeft(sprite, letter.transform).x;
        }

        if (i == 0 || lastLetterWasNewLine == true || lastLetterWasSpace == true)
        {
            float tightLeft = GetTightBottomLeft(sprite, letter.transform).x;
            float correction = 0;

            if(lastLetterWasNewLine == true || i == 0)
                {
                    correction = xLocation - tightLeft;   
                }
            else if(lastLetterWasSpace == true)
                {
                    correction = offset.x - tightLeft;  
                }

            letter.transform.position += new Vector3(correction, 0, 0);
            offset.x += correction;
        }
        
        float letterWidth = fontWidths[spriteName] / 2;
        if(i != input.Length - 1 && fontWidths.ContainsKey(char.ToUpper(input[i + 1]).ToString()))
        {
            letterWidth += fontWidths[char.ToUpper(input[i + 1]).ToString()] / 2f;   
        }

        offset += new Vector3((letterWidth + (LETTER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0, 0);
        lastLetterWasNewLine = false;
        lastLetterWasSpace = false;
    }

    textParent.transform.position = GetTightTopLeft(fontAtlas.GetSprite(letters[0].name), letters[0].transform);
    foreach(GameObject character in letters)
    {
        character.transform.SetParent(textParent.transform);
    }

    if (offset.x > maxTextWidth)
    {
        maxTextWidth = Mathf.Abs(xLocation - GetTightBottomRight(sprite, letter.transform).x); 
    }

    maxTextHeight = ((numberOfNewLines + 1) * lineHeight + (numberOfNewLines * NEW_LINE_OFFSET / TEXT_PPU)) * TEXT_SCALE;
}

private void GenerateTextBackground(float backgroundWidth, float backgroundHeight)
{
    if(backgroundWidth == 0 || backgroundHeight == 0 
    || isUsingBackground == false) return;

    DestroyImmediate(textBackground);
    
    textBackground = Instantiate(backgroundPrefab, gameObject.transform);
    Canvas backgroundCanvas = textBackground.GetComponent<Canvas>();
    RectTransform backgroundTransform = textBackground.GetComponent<RectTransform>();

    backgroundCanvas.sortingOrder = SORTING_ORDER - 1;
    backgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundWidth + BACKGROUND_BUFFER);
    backgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backgroundHeight + BACKGROUND_BUFFER);
    backgroundTransform.localPosition = 
    new Vector3(backgroundWidth, -backgroundHeight) / 2f;
}

}
