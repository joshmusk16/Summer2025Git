using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class TextElement : MonoBehaviour
{

[Header("Text Attributes")]
public string inputText;
public SpriteAtlas fontAtlas;
public GameObject prefabCharacter;

private List<GameObject> letters = new List<GameObject>();
private const float LETTER_OFFSET_LENGTH = 2f;
private const float SPACE_LENGTH = 1.25f;
private const int SORTING_ORDER = 100;
private const float TEXT_SCALE = 0.75f;
private const float NEW_LINE_OFFSET = 0.5f;
private const float TEXT_PPU = 16f;

[Header("Background Attributes")]
public GameObject backgroundPrefab;
public bool isUsingBackground = true;
private const float BACKGROUND_BUFFER = 1.5f;
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
        GenerateText(inputText);
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

public void GenerateText(string input)
{
    letters.Clear();
    Vector3 offset = Vector3.zero;
    int numberOfNewLines = 0;
    float lastLetterHalfWidth = 0;
    maxTextWidth = 0;
    maxTextHeight = 0;

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

                offset = new Vector3(0, -(lineHeight + NEW_LINE_OFFSET) * numberOfNewLines) * TEXT_SCALE;
                lastLetterHalfWidth = 0;
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
        
        float letterWidth = fontWidths[spriteName] / 2;
        if(i != input.Length - 1 && fontWidths.ContainsKey(char.ToUpper(input[i + 1]).ToString()))
        {
            letterWidth += fontWidths[char.ToUpper(input[i + 1]).ToString()] / 2f;   
        }

        offset += new Vector3((letterWidth + (LETTER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0, 0);
        lastLetterHalfWidth = fontWidths[char.ToUpper(input[i]).ToString()] / 2f;
    }

    if (offset.x > maxTextWidth)
    {
        maxTextWidth = offset.x + (lastLetterHalfWidth - LETTER_OFFSET_LENGTH / TEXT_PPU) * TEXT_SCALE;
    }

    maxTextHeight = ((numberOfNewLines + 1) * lineHeight + (numberOfNewLines * NEW_LINE_OFFSET)) * TEXT_SCALE;

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

    if (maxY < minY) return sprite.bounds.size.y; // fallback if fully transparent

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

    // Fallback if fully transparent
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

#endregion

}
