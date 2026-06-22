using UnityEngine;
using System;
using System.Collections.Generic;

public class ComboBarUI : MonoBehaviour
{

public GameObject emptyObject;
private List<GameObject> Digits = new();
private List<int> IntegerDigits = new();
public Sprite[] numberSprites = new Sprite[10];
private const int SPACE_LENGTH = 2;
private const float Y_OFFSET = 0.25f;

public static event Action<HitInfo> OnComboUpdate;

public void UpdateComboNumber(int number)
{
    DestroyDigits();
    AddDigitsToList(number);
    int numberOfDigits = CountDigits(number);
    Vector2 offset = new(0f, -Y_OFFSET);

    for(int i = 0; i < numberOfDigits; i++)
    {
        GameObject newDigit = Instantiate(emptyObject, (Vector2)gameObject.transform.position + offset, 
        Quaternion.identity, gameObject.transform);

        newDigit.name = IntegerDigits[i].ToString();
        Digits.Add(newDigit);
        newDigit.GetComponent<SpriteRenderer>().sprite = numberSprites[IntegerDigits[i]];
        offset += new Vector2((numberSprites[i].rect.width + SPACE_LENGTH) / 16f, 0);
    }

    //offset all numbers back to center
    foreach(GameObject digit in Digits)
    {
        digit.transform.position -= (Vector3)offset / 2f;
    }

    OnComboUpdate?.Invoke(null);
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
}

}
