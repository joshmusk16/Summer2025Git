using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CountElement : SpriteSampling
{

[Header("Input Attributes")]
private int leftNumber = 0;
private int rightNumber = 0;
public SpriteAtlas numberAtlas;
public GameObject prefabCharacter;

private List<GameObject> leftNumbers = new List<GameObject>();
private List<GameObject> rightNumbers = new List<GameObject>();

[Header("Parent GameObjects")]
private GameObject countElementParent;
private GameObject leftNumberParent;
private GameObject rightNumberParent;
private GameObject slashParent = null;
private GameObject slashObject = null;

private const float NUMBER_OFFSET_LENGTH = 2f;
private const float NUMBER_TO_SLASH_OFFSET = 3f;
private const float TEXT_SCALE = 0.4f;
private const float TEXT_PPU = 16f;
private const int SORTING_ORDER = 100;
private const string SLASH_NAME_IN_SPRITE_ATLAS = "f";

private Dictionary<string, float> numberWidths = new Dictionary<string, float>();

void Awake()
{
    FindFontWidths();
}

//Debugging only
void Update()
{
    if (Input.GetKeyDown(KeyCode.J))
    {
        UpdateNumber(1, 4, Vector2.zero);    
    }
}

void FindFontWidths()
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

private void GenerateNumberText(int inputNumber, ref GameObject numberParent, List<GameObject> numbers, bool isLeftNumber)
{
    string input = inputNumber.ToString();

    numbers.Clear();
    numberParent = Instantiate(prefabCharacter, gameObject.transform);

    Vector3 offset = Vector2.zero;
    GameObject number;
    Sprite sprite;

    for (int i = 0; i < input.Length; i++)
    {
        string spriteName = char.ToUpper(input[i]).ToString();
        sprite = numberAtlas.GetSprite(spriteName);
        
        number = Instantiate(prefabCharacter, offset, Quaternion.identity, gameObject.transform);
        number.name = spriteName;
        number.transform.localScale = Vector3.one * TEXT_SCALE;
        number.GetComponent<SpriteRenderer>().sprite = sprite;
        number.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;
        numbers.Add(number);

        float letterWidth = numberWidths[spriteName] / 2;
        if(i != input.Length - 1 && numberWidths.ContainsKey(char.ToUpper(input[i + 1]).ToString()))
        {
            letterWidth += numberWidths[char.ToUpper(input[i + 1]).ToString()] / 2f;   
        }

        offset += new Vector3((letterWidth + (NUMBER_OFFSET_LENGTH / TEXT_PPU)) * TEXT_SCALE, 0, 0);
        }

    if (isLeftNumber)
    {
        numberParent.transform.position = GetTightBottomRight(numberAtlas.GetSprite(numbers[^1].name), numbers[^1].transform);
        numberParent.name = "Left Number Parent";
    }
    else
    {
        numberParent.transform.position = GetTightBottomLeft(numberAtlas.GetSprite(numbers[0].name), numbers[0].transform);
        numberParent.name = "Right Number Parent";
    }
    
    foreach(GameObject character in numbers)
    {
        character.transform.SetParent(numberParent.transform);
    }
}

public void UpdateNumber(int newLeftNumber, int newRightNumber, Vector2 parentOffset)
{
    if(slashParent == null)
    {
        slashParent = Instantiate(prefabCharacter, gameObject.transform);
        slashObject = Instantiate(prefabCharacter, gameObject.transform);
        slashParent.name = "Slash Parent";

        slashObject.transform.localScale = Vector3.one * TEXT_SCALE;
        slashObject.name = "Forward Slash";
        slashObject.GetComponent<SpriteRenderer>().sprite = numberAtlas.GetSprite(SLASH_NAME_IN_SPRITE_ATLAS);
        slashObject.GetComponent<SpriteRenderer>().sortingOrder = SORTING_ORDER;

        slashParent.transform.position = GetTightBottomLeft(numberAtlas.GetSprite(SLASH_NAME_IN_SPRITE_ATLAS), slashObject.transform);
        slashObject.transform.SetParent(slashParent.transform);
   }

    if(newLeftNumber != leftNumber)
    {
        Destroy(leftNumberParent);
        leftNumber = newLeftNumber;
        GenerateNumberText(newLeftNumber, ref leftNumberParent, leftNumbers, true);
    }

    if(newRightNumber != rightNumber)
    {
        Destroy(rightNumberParent);
        rightNumber = newRightNumber;
        GenerateNumberText(newRightNumber, ref rightNumberParent, rightNumbers, false);
    }

    float xOffset = NUMBER_TO_SLASH_OFFSET * TEXT_SCALE / TEXT_PPU;
    leftNumberParent.transform.position = slashParent.transform.position - new Vector3(xOffset, 0 , 0);
    rightNumberParent.transform.position = (Vector3)GetTightBottomRight(numberAtlas.GetSprite(SLASH_NAME_IN_SPRITE_ATLAS), slashObject.transform) + new Vector3(xOffset, 0 , 0);

    if(countElementParent == null) SetCountElementParent(parentOffset);
}   

public void SetCountElementParent(Vector2 offset)
{
    if(slashParent == null || leftNumberParent == null || rightNumberParent == null) return;

    countElementParent = Instantiate(prefabCharacter, gameObject.transform);
    countElementParent.name = "Count Element";

    countElementParent.transform.position = gameObject.transform.position + new Vector3(numberAtlas.GetSprite(SLASH_NAME_IN_SPRITE_ATLAS).rect.width / 2f, 0 ,0) * TEXT_SCALE / TEXT_PPU;

    slashParent.transform.SetParent(countElementParent.transform);
    leftNumberParent.transform.SetParent(countElementParent.transform);
    rightNumberParent.transform.SetParent(countElementParent.transform);

    countElementParent.transform.position = gameObject.transform.position + ((Vector3)offset * TEXT_SCALE / TEXT_PPU);
}

}