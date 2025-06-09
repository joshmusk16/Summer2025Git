using UnityEngine;

public class NumberUI : MonoBehaviour
{

    public SpriteRenderer ones;
    public SpriteRenderer tens;

    public Sprite[] numberSprites = new Sprite[10];

    public void UpdateNumber(int updateAmount)
    {
        if (Mathf.Abs(updateAmount) < 100)
        {
        tens.sprite = numberSprites[Mathf.FloorToInt(updateAmount / 10)];
        ones.sprite = numberSprites[Mathf.FloorToInt(updateAmount % 10)];   
        }
    }
}
