using UnityEngine;

public class DashChargeLogic : MonoBehaviour
{

private int state = 0; //0 is uncharged, 1 is charged
public Sprite[] chargeSprites = new Sprite[2];
public SpriteRenderer sr; //assign own sprite renderer in inspector

public void ChangeState(int newState)
{
    state = newState;
    sr.sprite = chargeSprites[state];
}

}
