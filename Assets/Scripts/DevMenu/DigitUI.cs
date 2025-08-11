using UnityEngine;

public class DigitUI : MonoBehaviour
{
    public SpriteRenderer ones;
    public Sprite[] numberSprites = new Sprite[10];

    public void UpdateNumber(int updateAmount)
    {
        if (Mathf.Abs(updateAmount) < 10)
        {
        ones.sprite = numberSprites[Mathf.FloorToInt(updateAmount)];   
        }
    }
}
