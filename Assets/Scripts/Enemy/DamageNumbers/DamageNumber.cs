using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public GameObject digit;
    public GameObject parent;
    public float digitSpacing;
    public float verticalSpawnOffset = 1f;

    private GameObject[] digitObjects;

    void Start()
    {
        gameObject.GetComponent<LerpUIHandler>().OnLocationLerpFinish += DestroyNumber;
    }

    public void InstantiateDamageNumber(int damage)
    {
        ClearDigitObjects();

        int digitCount = GetDigitCount(damage);
        int[] digits = ExtractDigitsForCount(damage, digitCount);
        digitObjects = new GameObject[digitCount];

        for (int i = 0; i < digitCount; i++)
        {
            Vector3 spawnPos = parent.transform.position + new Vector3(0, verticalSpawnOffset) + (digitSpacing * i * new Vector3(1, 0));
            digitObjects[i] = Instantiate(digit, spawnPos, Quaternion.identity, gameObject.transform);
            digitObjects[i].GetComponent<DigitUI>().UpdateNumber(digits[i]);
        }

        gameObject.GetComponent<LerpUIHandler>().LocationLerp(parent.transform.position + new Vector3(0f, 1.2f), 10);
    }

    public void DestroyNumber()
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        gameObject.GetComponent<LerpUIHandler>().OnLocationLerpFinish -= DestroyNumber;
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
