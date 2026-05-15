using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TextElement : MonoBehaviour
{

[Header("Text Attributes")]
public string textInput;
public SpriteAtlas fontAtlas;
public GameObject prefabCharacter;

private GameObject textParent;

private List<GameObject> letters = new List<GameObject>();
private const float LETTER_OFFSET_LENGTH = 2f;
private const float SPACE_LENGTH = 1.25f;
private const int SORTING_ORDER = 100;
private const float TEXT_SCALE = 0.6f;
private const float NEW_LINE_OFFSET = 4f;
private const float TEXT_PPU = 16f;

[Header("Header Attributes")]
private GameObject headerParent;
private float headerWidth;
private float headerHeight;
private List<GameObject> headerLetters = new List<GameObject>();
public bool isUsingHeader;
public string headerInput;
private const float HEADER_SCALE = 0.8f;
private const float HEADER_LETTER_OFFSET_LENGTH = 2f;
private const float HEADER_NEW_LINE_OFFSET = 8f;

[Header("Background Attributes")]
public GameObject backgroundPrefab;
public bool isUsingBackground = true;
private const float BACKGROUND_BUFFER = 0f;
private float maxTextHeight;
private float maxTextWidth;

private Dictionary<string, float> fontWidths = new Dictionary<string, float>();
private float lineHeight = 0;

//Debugging Update Method
void Update()
{
    if (Input.GetKeyDown(KeyCode.I))
    {
        DestroyText();
        GenerateHeader(headerInput);
        GenerateText(textInput);
    } 
}

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

public void GenerateHeader(string headerInput)
{
    headerLetters.Clear();
    Destroy(headerParent);

    Vector3 offset = Vector2.zero;
    headerParent = Instantiate(prefabCharacter, gameObject.transform);
    headerParent.name = "Header";
    float lastLetterHalfWidth = 0;

    for (int i = 0; i < headerInput.Length; i++)
        {
            if (headerInput[i] == ' ')
            {
                offset += new Vector3(SPACE_LENGTH * HEADER_SCALE, 0, 0);
                continue;
            }

        string spriteName = char.ToUpper(headerInput[i]).ToString();
        Sprite sprite = fontAtlas.GetSprite(spriteName);
        GameObject letter = Instantiate(prefabCharacter, gameObject.transform.position + offset, Quaternion.identity, gameObject.transform);
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
        lastLetterHalfWidth = fontWidths[char.ToUpper(headerInput[i]).ToString()] / 2f;
        }

    headerParent.transform.position = GetTightTopLeft(fontAtlas.GetSprite(headerLetters[0].name), headerLetters[0].transform);
    foreach(GameObject letter in headerLetters)
    {
        letter.transform.SetParent(headerParent.transform);
    }

    offset += new Vector3(0, -(lineHeight * TEXT_SCALE) -(HEADER_NEW_LINE_OFFSET / TEXT_PPU * HEADER_SCALE));

    headerWidth = offset.x + (lastLetterHalfWidth - HEADER_LETTER_OFFSET_LENGTH / TEXT_PPU) * HEADER_SCALE;
    headerHeight = lineHeight * HEADER_SCALE;
}


public void GenerateText(string input)
{
    letters.Clear();
    Destroy(textParent);

    Vector3 offset = Vector3.zero;
    maxTextWidth = 0;
    maxTextHeight = 0;

    int numberOfNewLines = 0;

    float xLocation = 0;

    bool lasterLetterWasNewLine = false;
    float lastLetterHalfWidth = 0;

    textParent = Instantiate(prefabCharacter, gameObject.transform);
    textParent.name = "Text";

    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == ' ')
        {
            offset += new Vector3(SPACE_LENGTH * TEXT_SCALE, 0, 0);
            lastLetterHalfWidth = 0;
            continue;
        }
        else if(input[i] == '/' && i != input.Length - 1)
        {
            if(input[i + 1] == 'N')
            {
                i++;
                numberOfNewLines++;

                if(maxTextWidth < offset.x)
                {
                    maxTextWidth = offset.x + (lastLetterHalfWidth - LETTER_OFFSET_LENGTH / TEXT_PPU) * TEXT_SCALE;      
                }

                offset = new Vector3(xLocation, (-(lineHeight * numberOfNewLines + NEW_LINE_OFFSET / TEXT_PPU * numberOfNewLines)) * TEXT_SCALE);                
                lastLetterHalfWidth = 0;
                lasterLetterWasNewLine = true;
                continue;
            }
        }

        string spriteName = char.ToUpper(input[i]).ToString();
        Sprite sprite = fontAtlas.GetSprite(spriteName);

        GameObject letter = Instantiate(prefabCharacter, gameObject.transform.position + offset, Quaternion.identity, gameObject.transform);
        letter.name = spriteName;
        letter.transform.localScale = Vector3.one * TEXT_SCALE;
        letter.GetComponent<SpriteRenderer>().sprite = sprite;
        letter.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;
        letters.Add(letter);

        if(i == 0)
        {
            xLocation = GetTightBottomLeft(sprite, letter.transform).x;
        }

        if(i == 0 || lasterLetterWasNewLine == true)
        {
            offset = new Vector3(GetTightBottomLeft(sprite, letter.transform).x - offset.x, offset.y);
            letter.transform.position = offset;
        }
        
        float letterWidth = fontWidths[spriteName] / 2;
        if(i != input.Length - 1 && fontWidths.ContainsKey(char.ToUpper(input[i + 1]).ToString()))
        {
            letterWidth += fontWidths[char.ToUpper(input[i + 1]).ToString()] / 2f;   
        }

        offset += new Vector3((letterWidth + (LETTER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0, 0);
        lastLetterHalfWidth = fontWidths[char.ToUpper(input[i]).ToString()] / 2f;
        lasterLetterWasNewLine = false;
    }

    textParent.transform.position = GetTightTopLeft(fontAtlas.GetSprite(letters[0].name), letters[0].transform);
    foreach(GameObject letter in letters)
    {
        letter.transform.SetParent(textParent.transform);
    }

    if (offset.x > maxTextWidth)
    {
        maxTextWidth = offset.x + (lastLetterHalfWidth - LETTER_OFFSET_LENGTH / TEXT_PPU) * TEXT_SCALE;
    }

    maxTextHeight = ((numberOfNewLines + 1) * lineHeight + (numberOfNewLines * NEW_LINE_OFFSET / TEXT_PPU)) * TEXT_SCALE;

    if (isUsingBackground)
    {
        GenerateTextBackground();
    }
}

private void GenerateTextBackground()
{
    if(maxTextHeight == 0 || maxTextWidth == 0 
    || isUsingBackground == false) return;
    
    GameObject background = Instantiate(backgroundPrefab, gameObject.transform);
    Canvas backgroundCanvas = background.GetComponent<Canvas>();
    RectTransform backgroundTransform = background.GetComponent<RectTransform>();

    backgroundCanvas.sortingOrder = SORTING_ORDER - 1;
    backgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxTextWidth + BACKGROUND_BUFFER);
    backgroundTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxTextHeight + BACKGROUND_BUFFER);
    backgroundTransform.localPosition = GetTightTopLeft(letters[0].GetComponent<SpriteRenderer>().sprite, letters[0].transform) + 
    new Vector2(maxTextWidth, -maxTextHeight) / 2f;
    Debug.Log(GetTightTopLeft(letters[0].GetComponent<SpriteRenderer>().sprite, letters[0].transform));

}

private void DestroyText()
{
    foreach(GameObject letter in letters)
    {
        Destroy(letter);    
    }     
    letters.Clear();
}

#region Sprite Sampling Helper Methods

private float GetTightWidth(Sprite sprite)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x, 
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width, 
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int maxX = rect.xMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x);
            }

    if (maxX < minX)    
    {
        Debug.LogWarning($"GetTightWidth fallback triggered for: {sprite.name}, bounds.size.x: {sprite.bounds.size.x}");
        return sprite.bounds.size.x;
    }

    float tightWidthPixels = maxX - minX + 1;
    return tightWidthPixels / sprite.pixelsPerUnit;
}

private float GetTightHeight(Sprite sprite)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x, 
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width, 
        (int)sprite.textureRect.height
    );

    int minY = rect.yMax;
    int maxY = rect.yMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }

    if (maxY < minY) return sprite.bounds.size.y; //fallback if fully transparent

    float tightHeightPixels = maxY - minY + 1;
    return tightHeightPixels / sprite.pixelsPerUnit;
}

private Vector2 GetTightTopLeft(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int maxY = rect.yMin;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                maxY = Mathf.Max(maxY, y);
            }

    //Fallback if fully transparent
    if (minX > rect.xMax - 1 || maxY < rect.yMin)
    {
        Debug.LogWarning($"GetTightTopLeft fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.min.x, b.max.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (minX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (maxY - sprite.textureRect.y - sprite.pivot.y + 1) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

private Vector2 GetTightBottomLeft(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int minX = rect.xMax;
    int minY = rect.yMax;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
            }

    //Fallback if fully transparent
    if (minX > rect.xMax - 1 || minY > rect.yMax - 1)
    {
        Debug.LogWarning($"GetTightBottomLeft fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.min.x, b.min.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (minX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (minY - sprite.textureRect.y - sprite.pivot.y) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

private Vector2 GetTightBottomRight(Sprite sprite, Transform spriteTransform)
{
    Texture2D tex = sprite.texture;
    RectInt rect = new RectInt(
        (int)sprite.textureRect.x,
        (int)sprite.textureRect.y,
        (int)sprite.textureRect.width,
        (int)sprite.textureRect.height
    );

    int maxX = rect.xMin;
    int minY = rect.yMax;

    for (int x = rect.xMin; x < rect.xMax; x++)
        for (int y = rect.yMin; y < rect.yMax; y++)
            if (tex.GetPixel(x, y).a > 0.01f)
            {
                maxX = Mathf.Max(maxX, x);
                minY = Mathf.Min(minY, y);
            }

    //Fallback if fully transparent
    if (maxX < rect.xMin + 1 || minY > rect.yMax - 1)
    {
        Debug.LogWarning($"GetTightBottomRight fallback triggered for: {sprite.name}");
        Bounds b = sprite.bounds;
        Vector3 worldPos = spriteTransform.TransformPoint(new Vector3(b.max.x, b.min.y, 0));
        return new Vector2(worldPos.x, worldPos.y);
    }

    float localX = (maxX - sprite.textureRect.x - sprite.pivot.x) / sprite.pixelsPerUnit;
    float localY = (minY - sprite.textureRect.y - sprite.pivot.y) / sprite.pixelsPerUnit;

    //Transform local sprite space to world space
    Vector3 worldPoint = spriteTransform.TransformPoint(new Vector3(localX, localY, 0));
    return new Vector2(worldPoint.x, worldPoint.y);
}

#endregion

}
