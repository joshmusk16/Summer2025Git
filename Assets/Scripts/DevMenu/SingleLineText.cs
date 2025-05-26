using System.Collections.Generic;
using UnityEngine;

public class SingleLineText : MonoBehaviour
{
    public string word; 
    public Sprite[] fontSprites;
    public GameObject emptyLetter;
    public int spaceLength;
    public int sortingOrder;
    private List<GameObject> letters = new List<GameObject>();

    void Start()
    {
        GenerateSingleLineText();
    }

    public void GenerateSingleLineText()
    {
        letters.Clear();
        Vector3 offset = Vector3.zero;

        for (int i = 0; i < word.Length; i++)
        {
            int index = CorrectIndex(word[i]);

            GameObject letter = Instantiate(emptyLetter, gameObject.transform.position + offset, Quaternion.identity, gameObject.transform);
            letter.name = fontSprites[index].name;
            letter.GetComponent<SpriteRenderer>().sprite = fontSprites[index];
            letter.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            letters.Add(letter);
            offset += new Vector3((fontSprites[index].rect.width + spaceLength) / 16f, 0, 0);
        }

    }

    //This method can be modified in the future to correct more indices for more charcters
    //reference the ASCII Table to find a new index correction
    public int CorrectIndex(int index)
    {
        if (index > 64 && index < 91)
        {
            index -= 65;
        }
        return index;
    }

}
