using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public int debugNumber;

    public GameObject digit;
    public float digitSpacing;

    private GameObject[] digitObjects;

    //Update for debugging
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            InstantiateDamageNumber(debugNumber);
        }
    }
    

    public void InstantiateDamageNumber(int damage)
    {
        ClearDigitObjects();

        int digitCount = GetDigitCount(damage);
        int[] digits = ExtractDigitsForCount(damage, digitCount);
        digitObjects = new GameObject[digitCount];

        for (int i = 0; i < digitCount; i++)
        {
            Vector3 spawnPos = gameObject.transform.position + new Vector3(1, 0) * digitSpacing * i;
            digitObjects[i] = Instantiate(digit, spawnPos, Quaternion.identity, gameObject.transform);
            digitObjects[i].GetComponent<DigitUI>().UpdateNumber(digits[i]);
        }
    }

    int GetDigitCount(int number)
    {
        if (number == 0) return 1;
        if (number < 10) return 1;
        if (number < 100) return 2;
        return 3;
    }

    int[] ExtractDigitsForCount(int number, int digitCount)
    {
        int[] digits = new int[digitCount];

        if (number >= 999)
        {
            digits[0] = 9;
            digits[1] = 9;
            digits[2] = 9;
            return digits;
        }

        for (int i = digitCount - 1; i >= 0; i--)
        {
            digits[i] = number % 10;
            number /= 10;
        }

        return digits;
    }
    
     void ClearDigitObjects()
    {
        if (digitObjects != null)
        {
            for (int i = 0; i < digitObjects.Length; i++)
            {
                if (digitObjects[i] != null)
                {
                    DestroyImmediate(digitObjects[i]);
                }
            }
        }
    }
}
