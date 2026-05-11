using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TextElement : MonoBehaviour
{

public string inputText;
private List<GameObject> letters = new List<GameObject>();
public SpriteAtlas fontAtlas;
public GameObject prefabCharacter;

private const float LETTER_OFFSET_LENGTH = 0f;
private const float SPACE_LENGTH = 1f;
private const int SORTING_ORDER = 100;
private const float TEXT_SCALE = 0.6f;
private const float NEW_LINE_OFFSET = 1.5f;

public Dictionary<string, float> fontWidths = new Dictionary<string, float>();

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

public void GenerateText(string input)
{
    letters.Clear();
    Vector3 offset = Vector3.zero;
    int numberOfNewLines = 0;

    for (int i = 0; i < input.Length; i++)
    {
        if (input[i] == ' ')
        {
            offset += new Vector3((SPACE_LENGTH + (LETTER_OFFSET_LENGTH / 16f)) * TEXT_SCALE, 0, 0);
            continue;
        }
        else if(input[i] == '/' && i != input.Length - 1)
        {
            if(input[i + 1] == 'N')
            {
                i++;
                numberOfNewLines++;
                offset = new Vector3(0, -NEW_LINE_OFFSET * numberOfNewLines) * TEXT_SCALE;
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
        
        float letterWidth = fontWidths[spriteName];
        offset += new Vector3((letterWidth + (LETTER_OFFSET_LENGTH / 16f)) * TEXT_SCALE, 0, 0);
    }
}

private void DestroyText()
{
    foreach(GameObject letter in letters)
    {
        Destroy(letter);    
    }     
    letters.Clear();
}

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

    if (maxX < minX) return sprite.bounds.size.x; // fallback if fully transparent

    float tightWidthPixels = maxX - minX + 1;
    return tightWidthPixels / sprite.pixelsPerUnit;
}

}
